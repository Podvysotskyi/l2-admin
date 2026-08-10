using L2.Admin.Configurations;

var builder = WebApplication.CreateBuilder(args)
    .AddAdminApi("l2-admin-api");
builder.Services.AddAdminRepositories(builder.Configuration);
var app = builder.Build();
app.MapAdminApi();
app.Run();
