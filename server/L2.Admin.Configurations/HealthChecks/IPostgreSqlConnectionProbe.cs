namespace L2.Admin.Configurations.HealthChecks;

public interface IPostgreSqlConnectionProbe
{
    Task CheckAsync(CancellationToken cancellationToken);
}
