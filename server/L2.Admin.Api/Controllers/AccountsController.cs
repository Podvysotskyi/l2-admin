using L2.Admin.Contracts;
using L2.Admin.Api.Filters;
using L2.Admin.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace L2.Admin.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public sealed class AccountsController(IAccountDirectoryRepository repository) : ControllerBase
{
    [HttpGet]
    [ValidateAccountDirectoryRequest]
    public async Task<ActionResult<AccountDirectoryResponse>> SearchAsync(
        [FromQuery] AccountDirectoryRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await repository.SearchAsync(
            request.Query ?? string.Empty,
            request.Page,
            request.PageSize,
            cancellationToken));
    }
}
