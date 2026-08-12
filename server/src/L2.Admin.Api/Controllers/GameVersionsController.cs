using L2.Admin.Contracts;
using L2.Admin.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace L2.Admin.Api.Controllers;

[ApiController]
[Route("api/game-versions")]
public sealed class GameVersionsController(IGameVersionRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GameVersionSummary>>> ListAsync(
        CancellationToken cancellationToken = default) =>
        Ok(await repository.ListAsync(cancellationToken));
}
