using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace L2.Admin.Configurations;

public static class AdminApiConfigurationExtensions
{
    public static WebApplicationBuilder AddAdminApi(
        this WebApplicationBuilder builder,
        string serviceName)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddJsonConsole(options => options.IncludeScopes = true);
        builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Warning);
        builder.Logging.AddFilter("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogLevel.Warning);
        builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware", LogLevel.Information);
        builder.Services.AddHttpLogging(options => options.LoggingFields =
            HttpLoggingFields.RequestProperties |
            HttpLoggingFields.ResponseStatusCode |
            HttpLoggingFields.Duration);
        builder.Services.AddHttpLoggingInterceptor<HealthCheckHttpLoggingInterceptor>();
        builder.Services.AddHttpClient();
        builder.Services.AddHealthChecks();
        builder.Services.AddControllers();

        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
        {
            if (origins.Length > 0)
            {
                policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            }
        }));

        var buildVersion = Assembly.GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "0.1.0-local";
        builder.Services.AddSingleton(new ServiceIdentity(serviceName, buildVersion));
        return builder;
    }

    public static WebApplication MapAdminApi(this WebApplication app)
    {
        app.UseHttpLogging();
        app.UseCors();
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });
        app.MapControllers();
        return app;
    }
}
