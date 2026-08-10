using L2.Admin.Api.Controllers;
using L2.Admin.Configurations;
using L2.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace L2.Admin.Api.Tests;

public sealed class SystemControllerTests
{
    [Fact]
    public void GetInfo_returns_service_identity_and_environment()
    {
        var controller = new SystemController(
            new ServiceIdentity("l2-admin-api", "1.2.3"),
            new StubWebHostEnvironment { EnvironmentName = "Testing" });

        var result = controller.GetInfo();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var info = Assert.IsType<AdminServiceInfo>(ok.Value);
        Assert.Equal("l2-admin-api", info.Service);
        Assert.Equal("1.2.3", info.BuildVersion);
        Assert.Equal("Testing", info.Environment);
    }
}
