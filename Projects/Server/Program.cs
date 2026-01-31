var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/test", () => "hello world");

app.Run();
