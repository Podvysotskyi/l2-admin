using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace L2.Admin.Api.Tests;

internal sealed class StubWebHostEnvironment : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "L2.Admin.Api.Tests";
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string WebRootPath { get; set; } = string.Empty;
    public string EnvironmentName { get; set; } = "Testing";
    public string ContentRootPath { get; set; } = string.Empty;
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
