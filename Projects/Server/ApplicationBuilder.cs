using System.Text;
using Controllers;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Middlewares;
using Services;

public class ApplicationBuilder
{
    private WebApplicationBuilder WebApplicationBuilder;

    public ApplicationBuilder(string[] args)
    {
        this.WebApplicationBuilder = WebApplication.CreateBuilder(args);

        this.WebApplicationBuilder.Services.AddEndpointsApiExplorer();
        this.WebApplicationBuilder.Services.AddSwaggerGen(options =>
        {
            options.SupportNonNullableReferenceTypes();
        });
        var configuration = new Configuration();
        this.WebApplicationBuilder.Services.AddSingleton(configuration);
        this.WebApplicationBuilder.Services.AddSingleton<JwtService>();
        this.WebApplicationBuilder.Services.AddScoped<UserService>();
        this.WebApplicationBuilder.Services.AddScoped<MessageService>();
        this.WebApplicationBuilder.Services.AddScoped<AuthService>();

        this.WebApplicationBuilder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
        this.WebApplicationBuilder.Services.AddAuthorization();
    }

    public void AddDbContextPool()
    {
        this.WebApplicationBuilder.Services.AddDbContextPool<AppDbContext>((serviceProvider, optionsBuilder) =>
        {
            var configuration = serviceProvider.GetService<Configuration>()!;
            AppDbContextFactory.InitializeOptionsFromConfiguration(configuration, (DbContextOptionsBuilder<AppDbContext>) optionsBuilder);
        });
    }

    public void AddDbContext()
    {
        this.WebApplicationBuilder.Services.AddDbContext<AppDbContext>((serviceProvider, optionsBuilder) =>
        {
            var configuration = serviceProvider.GetService<Configuration>()!;
            AppDbContextFactory.InitializeOptionsFromConfiguration(configuration, (DbContextOptionsBuilder<AppDbContext>) optionsBuilder);
        });
    }

    public void UseUrl(string url)
    {
        this.WebApplicationBuilder.WebHost.UseUrls(url);
    }

    public WebApplication GetWebApplication(
        bool atomic = true
    )
    {
        var app = this.WebApplicationBuilder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        if (atomic)
        {
            app.UseMiddleware<TransactionMiddleware>();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGroup("/auth")
            .MapAuthEndpoints();

        app.MapGroup("/messages")
            .MapMessageEndpoints()
            .RequireAuthorization();

        return app;
    }
}