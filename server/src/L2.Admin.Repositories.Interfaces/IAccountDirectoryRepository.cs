using L2.Admin.Contracts;

namespace L2.Admin.Repositories.Interfaces;

public interface IAccountDirectoryRepository
{
    Task<AccountDirectoryResponse> SearchAsync(
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
