using L2.Admin.Contracts;
using L2.Admin.Api.Filters;
using L2.Admin.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace L2.Admin.Api.Controllers;

[ApiController]
[Route("api/characters")]
public sealed class CharactersController(ICharacterDirectoryRepository repository) : ControllerBase
{
    [HttpGet]
    [ValidateCharacterDirectoryRequest]
    public async Task<ActionResult<CharacterDirectoryResponse>> SearchAsync(
        [FromQuery] CharacterDirectoryRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await repository.SearchAsync(
            request.GameVersion,
            request.Query ?? string.Empty,
            request.Page,
            request.PageSize,
            cancellationToken));
    }
}
