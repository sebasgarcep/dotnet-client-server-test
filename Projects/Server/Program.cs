using Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Configuration>();
builder.Services.AddDbContextFactory<AppDbContext>((serviceProvider, optionsBuilder) =>
{
    var configuration = serviceProvider.GetService<Configuration>()!;
    AppDbContextFactory.InitializeOptionsFromConfiguration(configuration, (DbContextOptionsBuilder<AppDbContext>) optionsBuilder);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/test", () => "hello world");

app.Run();
