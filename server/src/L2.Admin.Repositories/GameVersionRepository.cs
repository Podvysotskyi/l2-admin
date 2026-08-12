using L2.Admin.Contracts;
using L2.Admin.Exceptions;
using L2.Admin.Repositories.Contracts;
using L2.Admin.Repositories.Interfaces;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace L2.Admin.Repositories;

public sealed class GameVersionRepository(NpgsqlDataSource dataSource) : IGameVersionRepository
{
    public async Task<IReadOnlyList<GameVersionSummary>> ListAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            using var database = new QueryFactory(connection, new PostgresCompiler());
            var rows = await database.Query("game_versions")
                .Select("key as Key", "display_name as DisplayName", "sort_order as SortOrder")
                .OrderBy("sort_order")
                .OrderBy("key")
                .GetAsync<GameVersionRow>(cancellationToken: cancellationToken);
            return rows.Select(row => new GameVersionSummary(
                row.Key,
                row.DisplayName,
                row.SortOrder,
                row.Key == "interlude")).ToArray();
        }
        catch (NpgsqlException exception)
        {
            throw new AdminRepositoryException("Game version query failed.", exception);
        }
    }
}
