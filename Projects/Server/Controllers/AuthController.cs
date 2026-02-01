using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Services;

namespace Controllers
{
    public static class AuthController
    {
        public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapPost("/signin", Signin);
            routeGroupBuilder.MapPost("/signup", Signup);
            return routeGroupBuilder;
        }

        private static async Task<Results<Ok<AuthResponseDTO>, UnauthorizedHttpResult>> Signin(
            AuthRequestDTO authRequestDTO,
            AuthService authService,
            JwtService jwtService
        ) {
            var user = await authService.SignIn(authRequestDTO.Email, authRequestDTO.Password);
            if (user == null)
            {
                return TypedResults.Unauthorized();
            }
            return TypedResults.Ok(new AuthResponseDTO { Token = jwtService.GenerateToken(user) });
        }

        private static async Task<Results<Ok<AuthResponseDTO>, UnauthorizedHttpResult>> Signup(
            AuthRequestDTO authRequestDTO,
            AuthService authService,
            JwtService jwtService
        ) {
            var user = await authService.SignUp(authRequestDTO.Email, authRequestDTO.Password);
            if (user == null)
            {
                return TypedResults.Unauthorized();
            }
            return TypedResults.Ok(new AuthResponseDTO { Token = jwtService.GenerateToken(user) });
        }

        public class AuthRequestDTO
        {
            public required string Email { get; set => field = value.ToLower(); }
            public required string Password { get; set; }
        }

        public class AuthResponseDTO
        {
            public required string Token { get; set; }
        }
    }
}