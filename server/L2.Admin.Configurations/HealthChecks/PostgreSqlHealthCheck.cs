using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace L2.Admin.Configurations.HealthChecks;

public sealed class PostgreSqlHealthCheck(IPostgreSqlConnectionProbe connectionProbe) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await connectionProbe.CheckAsync(cancellationToken);
            return HealthCheckResult.Healthy("PostgreSQL is reachable.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL is unavailable.", exception);
        }
    }
}
