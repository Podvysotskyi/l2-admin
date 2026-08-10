using System.Data;
using L2.Admin.Contracts;
using L2.Admin.ReadModel;
using Npgsql;

namespace L2.Admin.Api.Characters;

public sealed class CharacterDirectoryRepository(
    NpgsqlDataSource dataSource,
    TimeProvider timeProvider)
{
    public async Task<CharacterDirectoryPage> SearchAsync(
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var normalizedQuery = query.ToUpperInvariant();
        var offset = ((long)page - 1) * pageSize;
        var now = timeProvider.GetUtcNow();
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                WITH matching AS (
                    SELECT
                        character.id,
                        character.name,
                        character.account_id,
                        account.username,
                        character.player_race_id,
                        race.name AS race_name,
                        character.player_sex_id,
                        sex.name AS sex_name,
                        character.base_class_id,
                        base_class.name AS base_class_name,
                        character.active_class_id,
                        active_class.name AS active_class_name,
                        character.level,
                        character.experience,
                        character.created_at,
                        character.delete_after
                    FROM player.characters AS character
                    LEFT JOIN accounts AS account ON account.id = character.account_id
                    LEFT JOIN content.player_races AS race ON race.id = character.player_race_id
                    LEFT JOIN content.player_sexes AS sex ON sex.id = character.player_sex_id
                    LEFT JOIN content.player_classes AS base_class
                        ON base_class.id = character.base_class_id
                        AND base_class.player_race_id = character.player_race_id
                        AND base_class.player_sex_id = character.player_sex_id
                    LEFT JOIN content.player_classes AS active_class
                        ON active_class.id = character.active_class_id
                        AND active_class.player_race_id = character.player_race_id
                        AND active_class.player_sex_id = character.player_sex_id
                    WHERE @query = ''
                        OR strpos(character.normalized_name, @query) > 0
                        OR strpos(account.normalized_username, @query) > 0
                        OR strpos(account.normalized_email, @query) > 0
                ),
                page AS (
                    SELECT *
                    FROM matching
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
            command.Parameters.AddWithValue("offset", NpgsqlTypes.NpgsqlDbType.Bigint, offset);
            command.Parameters.AddWithValue("page_size", NpgsqlTypes.NpgsqlDbType.Integer, pageSize);

            var items = new List<CharacterSummary>();
            long total = 0;
            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleResult, cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                total = reader.GetInt64(0);
                if (reader.IsDBNull(1)) continue;
                DateTimeOffset? deleteAfter = reader.IsDBNull(16)
                    ? null
                    : reader.GetFieldValue<DateTimeOffset>(16);
                items.Add(new CharacterSummary(
                    reader.GetGuid(1),
                    reader.GetString(2),
                    reader.GetGuid(3),
                    reader.IsDBNull(4) ? null : reader.GetString(4),
                    reader.GetInt32(5),
                    reader.IsDBNull(6) ? null : reader.GetString(6),
                    reader.GetInt32(7),
                    reader.IsDBNull(8) ? null : reader.GetString(8),
                    reader.GetInt32(9),
                    reader.IsDBNull(10) ? null : reader.GetString(10),
                    reader.GetInt32(11),
                    reader.IsDBNull(12) ? null : reader.GetString(12),
                    reader.GetInt16(13),
                    reader.GetInt64(14),
                    reader.GetFieldValue<DateTimeOffset>(15),
                    deleteAfter,
                    deleteAfter is null ? "active" : deleteAfter <= now ? "deletion_expired" : "pending_deletion"));
            }

            return new CharacterDirectoryPage(items, total, page, pageSize);
        }
        catch (Exception exception) when (AdminReadModelPersistence.IsPersistenceFailure(exception))
        {
            throw AdminReadModelPersistence.Wrap("Character directory query failed.", exception);
        }
    }
}
