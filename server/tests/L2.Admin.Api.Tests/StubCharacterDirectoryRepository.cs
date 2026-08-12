using L2.Admin.Contracts;
using L2.Admin.Repositories.Interfaces;

namespace L2.Admin.Api.Tests;

internal sealed class StubCharacterDirectoryRepository(CharacterDirectoryResponse response)
    : ICharacterDirectoryRepository
{
    public string? Query { get; private set; }
    public string? GameVersion { get; private set; }
    public int Page { get; private set; }
    public int PageSize { get; private set; }
    public CancellationToken CancellationToken { get; private set; }

    public Task<CharacterDirectoryResponse> SearchAsync(
        string gameVersion,
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        GameVersion = gameVersion;
        Query = query;
        Page = page;
        PageSize = pageSize;
        CancellationToken = cancellationToken;
        return Task.FromResult(response);
    }
}
