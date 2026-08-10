using L2.Admin.Api.Filters;
using L2.Admin.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace L2.Admin.Api.Tests;

public sealed class ValidateCharacterDirectoryRequestAttributeTests
{
    [Fact]
    public void OnActionExecuting_trims_valid_query()
    {
        var request = new CharacterDirectoryRequest { Query = "  aria  " };
        using var services = CreateServices();
        var context = CreateContext(request, services);

        new ValidateCharacterDirectoryRequestAttribute().OnActionExecuting(context);

        Assert.Equal("aria", request.Query);
        Assert.Null(context.Result);
    }

    [Fact]
    public void OnActionExecuting_returns_all_validation_errors()
    {
        var request = new CharacterDirectoryRequest
        {
            Query = new string('a', 255),
            Page = 0,
            PageSize = 0
        };
        using var services = CreateServices();
        var context = CreateContext(request, services);

        new ValidateCharacterDirectoryRequestAttribute().OnActionExecuting(context);

        var badRequest = Assert.IsType<BadRequestObjectResult>(context.Result);
        var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);
        Assert.Equal("Search terms must contain 254 characters or fewer.", Assert.Single(problem.Errors["query"]));
        Assert.Equal("Page must be at least 1.", Assert.Single(problem.Errors["page"]));
        Assert.Equal("Page size must be between 1 and 100.", Assert.Single(problem.Errors["pageSize"]));
    }

    private static ServiceProvider CreateServices() =>
        new ServiceCollection()
            .AddLogging()
            .AddControllers()
            .Services
            .BuildServiceProvider();

    private static ActionExecutingContext CreateContext(
        CharacterDirectoryRequest request,
        IServiceProvider services)
    {
        var httpContext = new DefaultHttpContext { RequestServices = services };
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor(),
            new ModelStateDictionary());
        return new ActionExecutingContext(
            actionContext,
            [],
            new Dictionary<string, object?> { ["request"] = request },
            new object());
    }
}
