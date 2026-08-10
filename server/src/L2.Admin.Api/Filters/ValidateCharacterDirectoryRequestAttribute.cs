using L2.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace L2.Admin.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ValidateCharacterDirectoryRequestAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.ActionArguments.Values.OfType<CharacterDirectoryRequest>().SingleOrDefault();
        if (request is null)
        {
            return;
        }

        request.Query = request.Query?.Trim();
        var errors = new Dictionary<string, string[]>();
        if (request.Query?.Length > 254)
        {
            errors["query"] = ["Search terms must contain 254 characters or fewer."];
        }

        if (request.Page < 1)
        {
            errors["page"] = ["Page must be at least 1."];
        }

        if (request.PageSize is < 1 or > 100)
        {
            errors["pageSize"] = ["Page size must be between 1 and 100."];
        }

        if (errors.Count == 0)
        {
            return;
        }

        var problemDetailsFactory = context.HttpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>();
        var modelState = new ModelStateDictionary();
        foreach (var (key, messages) in errors)
        {
            foreach (var message in messages)
            {
                modelState.AddModelError(key, message);
            }
        }

        context.Result = new BadRequestObjectResult(problemDetailsFactory.CreateValidationProblemDetails(
            context.HttpContext,
            modelState));
    }
}
