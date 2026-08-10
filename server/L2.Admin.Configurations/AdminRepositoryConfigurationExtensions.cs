using L2.Admin.Configurations.HealthChecks;
using L2.Admin.Repositories;
using L2.Admin.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace L2.Admin.Configurations;

public static class AdminRepositoryConfigurationExtensions
{
    public static IServiceCollection AddAdminRepositories(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSql")
            ?? throw new InvalidOperationException("ConnectionStrings:PostgreSql is required.");
        services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        services.AddSingleton<IAccountDirectoryRepository, AccountDirectoryRepository>();
        services.AddSingleton<ICharacterDirectoryRepository, CharacterDirectoryRepository>();
        services.AddSingleton<IPostgreSqlConnectionProbe, PostgreSqlConnectionProbe>();
        services.AddHealthChecks()
            .AddCheck<PostgreSqlHealthCheck>("postgresql", tags: ["ready"]);
        return services;
    }
}
