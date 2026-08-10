using L2.Admin.Api.Controllers;
using L2.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace L2.Admin.Api.Tests;

public sealed class AccountsControllerTests
{
    [Fact]
    public async Task SearchAsync_delegates_to_repository_and_returns_response()
    {
        var expected = new AccountDirectoryResponse([], 42, 3, 10);
        var repository = new StubAccountDirectoryRepository(expected);
        var controller = new AccountsController(repository);
        using var cancellation = new CancellationTokenSource();

        var result = await controller.SearchAsync(new AccountDirectoryRequest
        {
            Query = "admin",
            Page = 3,
            PageSize = 10
        }, cancellation.Token);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
        Assert.Equal("admin", repository.Query);
        Assert.Equal(3, repository.Page);
        Assert.Equal(10, repository.PageSize);
        Assert.Equal(cancellation.Token, repository.CancellationToken);
    }

    [Fact]
    public async Task SearchAsync_replaces_null_query_with_empty_string()
    {
        var repository = new StubAccountDirectoryRepository(
            new AccountDirectoryResponse([], 0, 1, 25));
        var controller = new AccountsController(repository);

        await controller.SearchAsync(new AccountDirectoryRequest());

        Assert.Equal(string.Empty, repository.Query);
    }
}
