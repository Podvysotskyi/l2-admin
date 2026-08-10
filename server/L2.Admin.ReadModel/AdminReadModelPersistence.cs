using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace L2.Admin.ReadModel;

public static class AdminReadModelPersistence
{
    public static IServiceCollection AddAdminReadModel(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSql")
            ?? throw new InvalidOperationException("ConnectionStrings:PostgreSql is required.");
        services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
        return services;
    }

    public static bool IsPersistenceFailure(Exception exception) => exception is NpgsqlException;

    public static AdminReadModelException Wrap(string message, Exception exception) => new(message, exception);
}

public sealed class AdminReadModelException(string message, Exception innerException)
    : Exception(message, innerException);
