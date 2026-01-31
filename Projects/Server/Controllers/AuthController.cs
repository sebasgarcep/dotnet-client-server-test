using Microsoft.AspNetCore.Http.HttpResults;
using Services;

namespace Controllers
{
    public static class AuthController
    {
        public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapPost("/signin", SignInRequest);
            routeGroupBuilder.MapPost("/signup", SignUpRequest);
            return routeGroupBuilder;
        }

        public class AuthRequestDTO
        {
            public required string Email { get; set => field = field.ToLower(); }
            public required string Password;
        }

        public class AuthResponseDTO
        {
            public required string Token;
        }

        private static async Task<Results<Ok<AuthResponseDTO>, UnauthorizedHttpResult>> SignInRequest(
            AuthRequestDTO authRequestDTO,
            AuthService authService,
            JwtService jwtService
        )
        {
            var user = await authService.SignIn(authRequestDTO.Email, authRequestDTO.Password);
            if (user == null)
            {
                return TypedResults.Unauthorized();
            }
            return TypedResults.Ok(new AuthResponseDTO { Token = jwtService.GenerateToken(user) });
        }

        private static async Task<Results<Ok<AuthResponseDTO>, UnauthorizedHttpResult>> SignUpRequest(
            AuthRequestDTO authRequestDTO,
            AuthService authService,
            JwtService jwtService
        )
        {
            var user = await authService.SignUp(authRequestDTO.Email, authRequestDTO.Password);
            if (user == null)
            {
                return TypedResults.Unauthorized();
            }
            return TypedResults.Ok(new AuthResponseDTO { Token = jwtService.GenerateToken(user) });
        }
    }
}