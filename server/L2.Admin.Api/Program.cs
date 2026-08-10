using L2.Admin.Api.Accounts;
using L2.Admin.Api.Characters;
using L2.Admin.Foundation;
using L2.Admin.ReadModel;

var builder = WebApplication.CreateBuilder(args)
    .AddL2Foundation("l2-admin-api");
builder.Services.AddAdminReadModel(builder.Configuration);
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddSingleton<AccountDirectoryRepository>();
builder.Services.AddSingleton<CharacterDirectoryRepository>();
var app = builder.Build();
app.MapL2Foundation();
app.MapAccountDirectory(app.Environment);
app.MapCharacterDirectory(app.Environment);
app.Run();

namespace L2.Admin.Api { public sealed class AdminApiMarker; }
