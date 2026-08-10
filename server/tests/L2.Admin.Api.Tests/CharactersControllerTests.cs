using L2.Admin.Api.Controllers;
using L2.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace L2.Admin.Api.Tests;

public sealed class CharactersControllerTests
{
    [Fact]
    public async Task SearchAsync_delegates_to_repository_and_returns_response()
    {
        var expected = new CharacterDirectoryResponse([], 21, 2, 50);
        var repository = new StubCharacterDirectoryRepository(expected);
        var controller = new CharactersController(repository);
        using var cancellation = new CancellationTokenSource();

        var result = await controller.SearchAsync(new CharacterDirectoryRequest
        {
            Query = "aria",
            Page = 2,
            PageSize = 50
        }, cancellation.Token);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
        Assert.Equal("aria", repository.Query);
        Assert.Equal(2, repository.Page);
        Assert.Equal(50, repository.PageSize);
        Assert.Equal(cancellation.Token, repository.CancellationToken);
    }

    [Fact]
    public async Task SearchAsync_replaces_null_query_with_empty_string()
    {
        var repository = new StubCharacterDirectoryRepository(
            new CharacterDirectoryResponse([], 0, 1, 25));
        var controller = new CharactersController(repository);

        await controller.SearchAsync(new CharacterDirectoryRequest());

        Assert.Equal(string.Empty, repository.Query);
    }
}
