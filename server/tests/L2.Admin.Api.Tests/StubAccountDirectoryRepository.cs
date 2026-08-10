using L2.Admin.Contracts;
using L2.Admin.Repositories.Interfaces;

namespace L2.Admin.Api.Tests;

internal sealed class StubAccountDirectoryRepository(AccountDirectoryResponse response)
    : IAccountDirectoryRepository
{
    public string? Query { get; private set; }
    public int Page { get; private set; }
    public int PageSize { get; private set; }
    public CancellationToken CancellationToken { get; private set; }

    public Task<AccountDirectoryResponse> SearchAsync(
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        Query = query;
        Page = page;
        PageSize = pageSize;
        CancellationToken = cancellationToken;
        return Task.FromResult(response);
    }
}
