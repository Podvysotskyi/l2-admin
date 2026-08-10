using L2.Admin.Configurations;
using L2.Admin.Repositories;
using L2.Admin.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace L2.Admin.Configurations.Tests;

public sealed class AdminRepositoryConfigurationExtensionsTests
{
    [Fact]
    public void AddAdminRepositories_requires_connection_string()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddAdminRepositories(configuration));

        Assert.Equal("ConnectionStrings:PostgreSql is required.", exception.Message);
    }

    [Fact]
    public void AddAdminRepositories_registers_repositories()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSql"] =
                    "Host=localhost;Database=l2web;Username=l2web;Password=l2web_dev"
            })
            .Build();

        var result = services.AddAdminRepositories(configuration);
        using var provider = services.BuildServiceProvider();

        Assert.Same(services, result);
        Assert.NotNull(provider.GetRequiredService<NpgsqlDataSource>());
        Assert.Same(TimeProvider.System, provider.GetRequiredService<TimeProvider>());
        Assert.IsType<AccountDirectoryRepository>(
            provider.GetRequiredService<IAccountDirectoryRepository>());
        Assert.IsType<CharacterDirectoryRepository>(
            provider.GetRequiredService<ICharacterDirectoryRepository>());
    }
}
