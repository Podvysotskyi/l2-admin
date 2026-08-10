using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace L2.Admin.Api.Accounts;

public static class AccountDirectoryEndpoints
{
    public static IEndpointRouteBuilder MapAccountDirectory(
        this IEndpointRouteBuilder endpoints,
        IWebHostEnvironment environment)
    {
        if (!environment.IsDevelopment() && !environment.IsEnvironment("Testing"))
        {
            return endpoints;
        }

        endpoints.MapGet("/api/accounts", SearchAsync);
        return endpoints;
    }

    private static async Task<IResult> SearchAsync(
        HttpContext context,
        AccountDirectoryRepository repository,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        [FromQuery] string? query = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        if (environment.IsDevelopment() &&
            !configuration.GetValue<bool>("Admin:AllowContainerAccess") &&
            (context.Connection.RemoteIpAddress is null || !IPAddress.IsLoopback(context.Connection.RemoteIpAddress)))
        {
            return Results.StatusCode(StatusCodes.Status403Forbidden);
        }

        query = query?.Trim() ?? string.Empty;
        var errors = new Dictionary<string, string[]>();
        if (query.Length > 254)
        {
            errors["query"] = ["Search terms must contain 254 characters or fewer."];
        }

        if (page < 1)
        {
            errors["page"] = ["Page must be at least 1."];
        }

        if (pageSize is < 1 or > 100)
        {
            errors["pageSize"] = ["Page size must be between 1 and 100."];
        }

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        return Results.Ok(await repository.SearchAsync(query, page, pageSize, cancellationToken));
    }
}
