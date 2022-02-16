using test.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterModules();

var app = builder.Build();
app.MapEndpoints();
app.Run();