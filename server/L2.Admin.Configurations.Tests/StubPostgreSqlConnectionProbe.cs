using L2.Admin.Configurations.HealthChecks;

namespace L2.Admin.Configurations.Tests;

internal sealed class StubPostgreSqlConnectionProbe(Exception? exception = null)
    : IPostgreSqlConnectionProbe
{
    public CancellationToken CancellationToken { get; private set; }

    public Task CheckAsync(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        return exception is null
            ? Task.CompletedTask
            : Task.FromException(exception);
    }
}
