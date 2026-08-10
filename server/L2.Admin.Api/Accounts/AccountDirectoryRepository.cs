using System.Data;
using L2.Admin.Contracts;
using L2.Admin.ReadModel;
using Npgsql;

namespace L2.Admin.Api.Accounts;

public sealed class AccountDirectoryRepository(
    NpgsqlDataSource dataSource,
    TimeProvider timeProvider)
{
    public async Task<AccountDirectoryPage> SearchAsync(
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var normalizedQuery = query.ToUpperInvariant();
        var offset = ((long)page - 1) * pageSize;
        var now = timeProvider.GetUtcNow();
        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = """
                WITH matching AS (
                    SELECT
                        account.id,
                        account.username,
                        account.email,
                        account.created_at,
                        (SELECT max(history.occurred_at)
                         FROM account_login_history AS history
                         WHERE history.account_id = account.id AND history.succeeded) AS last_successful_login_at,
                        EXISTS (
                            SELECT 1 FROM account_sessions AS session
                            WHERE session.account_id = account.id
                                AND session.revoked_at IS NULL
                                AND session.expires_at > @now) AS has_active_login_session
                    FROM accounts AS account
                    WHERE @query = ''
                        OR strpos(account.normalized_username, @query) > 0
                        OR strpos(account.normalized_email, @query) > 0
                ),
                page AS (
                    SELECT * FROM matching
                    ORDER BY created_at DESC, id DESC
                    OFFSET @offset
                    LIMIT @page_size
                )
                SELECT total.value, page.*
                FROM (SELECT count(*) AS value FROM matching) AS total
                LEFT JOIN page ON TRUE
                ORDER BY page.created_at DESC NULLS LAST, page.id DESC NULLS LAST;
                """;
            command.Parameters.AddWithValue("query", NpgsqlTypes.NpgsqlDbType.Text, normalizedQuery);
            command.Parameters.AddWithValue("now", NpgsqlTypes.NpgsqlDbType.TimestampTz, now);
            command.Parameters.AddWithValue("offset", NpgsqlTypes.NpgsqlDbType.Bigint, offset);
            command.Parameters.AddWithValue("page_size", NpgsqlTypes.NpgsqlDbType.Integer, pageSize);

            var accounts = new List<AccountSummary>();
            long total = 0;
            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleResult, cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                total = reader.GetInt64(0);
                if (reader.IsDBNull(1)) continue;
                accounts.Add(new AccountSummary(
                    reader.GetGuid(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetFieldValue<DateTimeOffset>(4),
                    reader.IsDBNull(5) ? null : reader.GetFieldValue<DateTimeOffset>(5),
                    reader.GetBoolean(6)));
            }
            return new AccountDirectoryPage(accounts, total, page, pageSize);
        }
        catch (Exception exception) when (AdminReadModelPersistence.IsPersistenceFailure(exception))
        {
            throw AdminReadModelPersistence.Wrap("Account directory query failed.", exception);
        }
    }
}
