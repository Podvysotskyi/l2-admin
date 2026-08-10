using Npgsql;

namespace L2.Admin.Configurations.HealthChecks;

public sealed class PostgreSqlConnectionProbe(NpgsqlDataSource dataSource)
    : IPostgreSqlConnectionProbe
{
    public async Task CheckAsync(CancellationToken cancellationToken)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
    }
}
