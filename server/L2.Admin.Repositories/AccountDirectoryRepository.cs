using L2.Admin.Contracts;
using L2.Admin.Exceptions;
using L2.Admin.Repositories.Contracts;
using L2.Admin.Repositories.Interfaces;
using Npgsql;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace L2.Admin.Repositories;

public sealed class AccountDirectoryRepository(
    NpgsqlDataSource dataSource,
    TimeProvider timeProvider) : IAccountDirectoryRepository
{
    public async Task<AccountDirectoryResponse> SearchAsync(
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var normalizedQuery = query.ToUpperInvariant();
        var now = timeProvider.GetUtcNow();

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            using var database = new QueryFactory(connection, new PostgresCompiler());
            var directoryQuery = BuildQuery(database.Query("accounts as account"), normalizedQuery, now);

            var result = await directoryQuery.PaginateAsync<AccountRow>(
                page,
                pageSize,
                cancellationToken: cancellationToken);
            var accounts = result.List.Select(MapAccount).ToArray();
            return new AccountDirectoryResponse(accounts, result.Count, page, pageSize);
        }
        catch (NpgsqlException exception)
        {
            throw new AdminRepositoryException("Account directory query failed.", exception);
        }
    }

    internal static Query BuildQuery(Query directoryQuery, string normalizedQuery, DateTimeOffset now)
    {
        var lastSuccessfulLogin = new Query("account_login_history as history")
            .WhereColumns("history.account_id", "=", "account.id")
            .WhereTrue("history.succeeded")
            .AsMax("history.occurred_at");
        var activeLoginSessionCount = new Query("account_sessions as session")
            .WhereColumns("session.account_id", "=", "account.id")
            .WhereNull("session.revoked_at")
            .Where("session.expires_at", ">", now)
            .AsCount();

        directoryQuery
            .Select(
                "account.id as Id",
                "account.username as Username",
                "account.email as Email",
                "account.created_at as CreatedAt")
            .Select(lastSuccessfulLogin, "LastSuccessfulLoginAt")
            .Select(activeLoginSessionCount, "ActiveLoginSessionCount")
            .OrderByDesc("account.created_at")
            .OrderByDesc("account.id");

        if (normalizedQuery.Length > 0)
        {
            var escapedQuery = EscapeLikePattern(normalizedQuery);
            directoryQuery.Where(filter => filter
                .WhereContains("account.normalized_username", escapedQuery, true, "\\")
                .OrWhereContains("account.normalized_email", escapedQuery, true, "\\"));
        }

        return directoryQuery;
    }

    internal static AccountSummary MapAccount(AccountRow account) => new(
        account.Id,
        account.Username,
        account.Email,
        new DateTimeOffset(account.CreatedAt),
        account.LastSuccessfulLoginAt is null
            ? null
            : new DateTimeOffset(account.LastSuccessfulLoginAt.Value),
        account.ActiveLoginSessionCount > 0);

    private static string EscapeLikePattern(string value) => value
        .Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace("%", "\\%", StringComparison.Ordinal)
        .Replace("_", "\\_", StringComparison.Ordinal);
}
