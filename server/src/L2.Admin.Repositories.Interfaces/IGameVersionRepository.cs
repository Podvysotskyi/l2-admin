using L2.Admin.Contracts;

namespace L2.Admin.Repositories.Interfaces;

public interface IGameVersionRepository
{
    Task<IReadOnlyList<GameVersionSummary>> ListAsync(CancellationToken cancellationToken);
}
