using Controllers;
using Data;
using Microsoft.EntityFrameworkCore;
using Services;
using Server.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Configuration>();
builder.Services.AddDbContextPool<AppDbContext>((serviceProvider, optionsBuilder) =>
{
    var configuration = serviceProvider.GetService<Configuration>()!;
    AppDbContextFactory.InitializeOptionsFromConfiguration(configuration, (DbContextOptionsBuilder<AppDbContext>) optionsBuilder);
});
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<TransactionMiddleware>();

app.MapGroup("/auth")
    .MapAuthEndpoints();

app.Run();
