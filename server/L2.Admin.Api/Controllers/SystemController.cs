using L2.Admin.Configurations;
using L2.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace L2.Admin.Api.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController(
    ServiceIdentity identity,
    IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet("info")]
    public ActionResult<AdminServiceInfo> GetInfo() =>
        Ok(new AdminServiceInfo(
            identity.Name,
            identity.BuildVersion,
            environment.EnvironmentName));
}
