using L2.Admin.Configurations.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace L2.Admin.Configurations.Tests;

public sealed class PostgreSqlHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_returns_healthy_when_probe_succeeds()
    {
        var probe = new StubPostgreSqlConnectionProbe();
        var healthCheck = new PostgreSqlHealthCheck(probe);
        using var cancellation = new CancellationTokenSource();

        var result = await healthCheck.CheckHealthAsync(
            new HealthCheckContext(),
            cancellation.Token);

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Equal("PostgreSQL is reachable.", result.Description);
        Assert.Equal(cancellation.Token, probe.CancellationToken);
    }

    [Fact]
    public async Task CheckHealthAsync_returns_unhealthy_when_probe_fails()
    {
        var expectedException = new InvalidOperationException("Connection failed.");
        var healthCheck = new PostgreSqlHealthCheck(
            new StubPostgreSqlConnectionProbe(expectedException));

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("PostgreSQL is unavailable.", result.Description);
        Assert.Same(expectedException, result.Exception);
    }
}
