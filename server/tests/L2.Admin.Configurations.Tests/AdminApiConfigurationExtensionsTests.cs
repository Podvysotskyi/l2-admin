using L2.Admin.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace L2.Admin.Configurations.Tests;

public sealed class AdminApiConfigurationExtensionsTests
{
    [Fact]
    public async Task AddAdminApi_registers_service_identity_and_api_services()
    {
        var builder = CreateBuilder();

        var result = builder.AddAdminApi("l2-admin-api");
        await using var app = builder.Build();

        Assert.Same(builder, result);
        var identity = app.Services.GetRequiredService<ServiceIdentity>();
        Assert.Equal("l2-admin-api", identity.Name);
        Assert.False(string.IsNullOrWhiteSpace(identity.BuildVersion));
        Assert.NotNull(app.Services.GetRequiredService<IHttpClientFactory>());
    }

    [Fact]
    public async Task AddAdminApi_configures_allowed_cors_origins()
    {
        var builder = CreateBuilder();
        builder.Configuration["Cors:AllowedOrigins:0"] = "https://admin.example.com";
        builder.Configuration["Cors:AllowedOrigins:1"] = "https://support.example.com";

        builder.AddAdminApi("l2-admin-api");
        await using var app = builder.Build();

        var options = app.Services.GetRequiredService<IOptions<CorsOptions>>().Value;
        var policy = Assert.IsType<CorsPolicy>(options.GetPolicy(options.DefaultPolicyName));
        Assert.Equal(
            ["https://admin.example.com", "https://support.example.com"],
            policy.Origins);
        Assert.Contains("*", policy.Headers);
        Assert.Contains("*", policy.Methods);
        Assert.True(policy.SupportsCredentials);
    }

    [Fact]
    public async Task AddAdminApi_configures_http_request_logging()
    {
        var builder = CreateBuilder();
        builder.AddAdminApi("l2-admin-api");
        await using var app = builder.Build();

        var options = app.Services.GetRequiredService<IOptions<HttpLoggingOptions>>().Value;

        Assert.Equal(
            HttpLoggingFields.RequestProperties |
            HttpLoggingFields.ResponseStatusCode |
            HttpLoggingFields.Duration,
            options.LoggingFields);
        Assert.Single(app.Services.GetServices<IHttpLoggingInterceptor>());
    }

    [Fact]
    public async Task MapAdminApi_maps_live_health_endpoint()
    {
        var builder = CreateBuilder();
        builder.AddAdminApi("l2-admin-api");
        await using var app = builder.Build();

        var result = app.MapAdminApi();

        Assert.Same(app, result);
        var routes = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>()
            .Select(endpoint => endpoint.RoutePattern.RawText)
            .ToArray();
        Assert.Contains("/health/live", routes);
        Assert.DoesNotContain("/health/ready", routes);
    }

    private static WebApplicationBuilder CreateBuilder() =>
        WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(AdminApiConfigurationExtensionsTests).Assembly.FullName,
            EnvironmentName = "Testing"
        });
}
