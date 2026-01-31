using Data;
using Controllers;
using Middlewares;
using Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = new Configuration();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
});
builder.Services.AddSingleton(configuration);
builder.Services.AddDbContextPool<AppDbContext>((serviceProvider, optionsBuilder) =>
{
    var configuration = serviceProvider.GetService<Configuration>()!;
    AppDbContextFactory.InitializeOptionsFromConfiguration(configuration, (DbContextOptionsBuilder<AppDbContext>) optionsBuilder);
});
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.JwtSecret))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<TransactionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/auth")
    .MapAuthEndpoints();

app.MapGroup("/messages")
    .MapMessageEndpoints()
    .RequireAuthorization();

app.Run();
