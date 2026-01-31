using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Services;

namespace Controllers
{
    public static class AuthController
    {
        public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder
                .MapPost("/signin", async Task<IResult> (
                    AuthRequestDTO authRequestDTO,
                    AuthService authService,
                    JwtService jwtService
                ) => {
                    var user = await authService.SignIn(authRequestDTO.Email, authRequestDTO.Password);
                    if (user == null)
                    {
                        return Results.Unauthorized();
                    }
                    return Results.Ok(new AuthResponseDTO { Token = jwtService.GenerateToken(user) });
                })
                .Produces<AuthResponseDTO>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);

            routeGroupBuilder
                .MapPost("/signup", async Task<IResult> (
                    AuthRequestDTO authRequestDTO,
                    AuthService authService,
                    JwtService jwtService
                ) => {
                    var user = await authService.SignUp(authRequestDTO.Email, authRequestDTO.Password);
                    if (user == null)
                    {
                        return Results.Unauthorized();
                    }
                    return Results.Ok(new AuthResponseDTO { Token = jwtService.GenerateToken(user) });
                })
                .Produces<AuthResponseDTO>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);

            return routeGroupBuilder;
        }

        public class AuthRequestDTO
        {
            public required string Email { get; set => field = field.ToLower(); }
            public required string Password { get; set; }
        }

        public class AuthResponseDTO
        {
            public required string Token { get; set; }
        }
    }
}