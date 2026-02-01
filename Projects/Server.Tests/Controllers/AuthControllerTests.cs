using System.Net;
using System.Net.Http.Json;
using Controllers;

namespace Server.Tests;

public class AuthControllerTests : IntegrationTestFixture
{
    public AuthControllerTests(ApplicationFixture applicationFixture) : base(applicationFixture)
    {
    }

    [Fact]
    public async Task SignUp_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var request = new AuthController.AuthRequestDTO
        {
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/signup", request);

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
        var request = new AuthController.AuthRequestDTO
        {
            Email = "duplicate@example.com",
            Password = "password123"
        };

        // First signup
        await Client.PostAsJsonAsync("/auth/signup", request);

        // Act - Second signup with same email
        var response = await Client.PostAsJsonAsync("/auth/signup", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SignIn_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange

        // First create a user
        var signupRequest = new AuthController.AuthRequestDTO
        {
            Email = "signin@example.com",
            Password = "password123"
        };
        await Client.PostAsJsonAsync("/auth/signup", signupRequest);

        // Act - Sign in
        var signinRequest = new AuthController.AuthRequestDTO
        {
            Email = "signin@example.com",
            Password = "password123"
        };
        var response = await Client.PostAsJsonAsync("/auth/signin", signinRequest);

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
        var request = new AuthController.AuthRequestDTO
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/signin", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SignIn_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange

        // First create a user
        var signupRequest = new AuthController.AuthRequestDTO
        {
            Email = "wrongpass@example.com",
            Password = "password123"
        };
        await Client.PostAsJsonAsync("/auth/signup", signupRequest);

        // Act - Sign in with wrong password
        var signinRequest = new AuthController.AuthRequestDTO
        {
            Email = "wrongpass@example.com",
            Password = "wrongpassword"
        };
        var response = await Client.PostAsJsonAsync("/auth/signin", signinRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SignUp_EmailIsLowercased()
    {
        // Arrange
        var request = new AuthController.AuthRequestDTO
        {
            Email = "UPPERCASE@EXAMPLE.COM",
            Password = "password123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/signup", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Try to sign in with lowercase email
        var signinRequest = new AuthController.AuthRequestDTO
        {
            Email = "uppercase@example.com",
            Password = "password123"
        };
        var signinResponse = await Client.PostAsJsonAsync("/auth/signin", signinRequest);
        Assert.Equal(HttpStatusCode.OK, signinResponse.StatusCode);
    }
}
