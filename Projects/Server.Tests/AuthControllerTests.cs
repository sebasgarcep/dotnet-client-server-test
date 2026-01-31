using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Data;
using Controllers;

namespace Server.Tests;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove all DbContext related services
                var descriptorsToRemove = services.Where(d =>
                    d.ServiceType == typeof(AppDbContext) ||
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(IDbContextFactory<AppDbContext>) ||
                    d.ServiceType.Name.Contains("DbContext")).ToList();

                foreach (var descriptor in descriptorsToRemove)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning));
                });

                // Override Configuration with test values
                var configDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(Configuration));
                if (configDescriptor != null)
                {
                    services.Remove(configDescriptor);
                }
                services.AddSingleton<Configuration>(new TestConfiguration());
            });
        });
    }

    [Fact]
    public async Task SignUp_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new AuthController.AuthRequestDTO
        {
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/auth/signup", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthController.AuthResponseDTO>();
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task SignUp_DuplicateEmail_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new AuthController.AuthRequestDTO
        {
            Email = "duplicate@example.com",
            Password = "password123"
        };

        // First signup
        await client.PostAsJsonAsync("/auth/signup", request);

        // Act - Second signup with same email
        var response = await client.PostAsJsonAsync("/auth/signup", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SignIn_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var client = _factory.CreateClient();

        // First create a user
        var signupRequest = new AuthController.AuthRequestDTO
        {
            Email = "signin@example.com",
            Password = "password123"
        };
        await client.PostAsJsonAsync("/auth/signup", signupRequest);

        // Act - Sign in
        var signinRequest = new AuthController.AuthRequestDTO
        {
            Email = "signin@example.com",
            Password = "password123"
        };
        var response = await client.PostAsJsonAsync("/auth/signin", signinRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthController.AuthResponseDTO>();
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task SignIn_InvalidEmail_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new AuthController.AuthRequestDTO
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/auth/signin", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SignIn_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // First create a user
        var signupRequest = new AuthController.AuthRequestDTO
        {
            Email = "wrongpass@example.com",
            Password = "password123"
        };
        await client.PostAsJsonAsync("/auth/signup", signupRequest);

        // Act - Sign in with wrong password
        var signinRequest = new AuthController.AuthRequestDTO
        {
            Email = "wrongpass@example.com",
            Password = "wrongpassword"
        };
        var response = await client.PostAsJsonAsync("/auth/signin", signinRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SignUp_EmailIsLowercased()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new AuthController.AuthRequestDTO
        {
            Email = "UPPERCASE@EXAMPLE.COM",
            Password = "password123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/auth/signup", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Try to sign in with lowercase email
        var signinRequest = new AuthController.AuthRequestDTO
        {
            Email = "uppercase@example.com",
            Password = "password123"
        };
        var signinResponse = await client.PostAsJsonAsync("/auth/signin", signinRequest);
        Assert.Equal(HttpStatusCode.OK, signinResponse.StatusCode);
    }
}

public class TestConfiguration : Configuration
{
    public TestConfiguration()
    {
        Environment.SetEnvironmentVariable("JWT_SECRET", "test-jwt-secret-key-for-testing-purposes-only");
    }
}
