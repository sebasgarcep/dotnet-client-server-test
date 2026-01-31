using Services;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Controllers
{
    public static class MessageController
    {
        public static RouteGroupBuilder MapMessageEndpoints(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder
                .MapGet("/", [Authorize] async Task<IResult> (
                    ClaimsPrincipal claimsPrincipal,
                    MessageService messageService,
                    UserService userService
                ) => {
                    var user = await userService.GetByClaimsPrincipal(claimsPrincipal);
                    if (user == null)
                    {
                        return Results.Unauthorized();
                    }
    
                    var messages = await messageService.GetAllMessagesOrderedByCreationTime(user);
                    var messageDTOs = messages.Select(m => new MessageResponseDTO(m)).ToList();
                    return Results.Ok(new MessageListResponseDTO { messages = messageDTOs });
                })
                .Produces<MessageListResponseDTO>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);

            routeGroupBuilder
                .MapPost("/", [Authorize] async Task<IResult> (
                    ClaimsPrincipal claimsPrincipal,
                    MessageRequestDTO messageRequest,
                    UserService userService,
                    MessageService messageService
                ) => {
                    var user = await userService.GetByClaimsPrincipal(claimsPrincipal);
                    if (user == null)
                    {
                        return Results.Unauthorized();
                    }

                    var message = new Message {
                        Text = messageRequest.Text,
                        User = user,
                    };
                    message = await messageService.AddMessage(message);
                    return Results.Ok(new MessageResponseDTO(message));
                })
                .Produces<MessageResponseDTO>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status401Unauthorized);

            return routeGroupBuilder;
        }

        public class MessageRequestDTO
        {
            public required string Text { get; set; }
        }

        public class MessageResponseDTO
        {
            public MessageResponseDTO(Message message)
            {
                this.Id = message.Id;
                this.Text = message.Text;
                this.CreatedAt = message.CreatedAt;
            }

            public int Id { get; set; }
            public string Text { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class MessageListResponseDTO
        {
            public required List<MessageResponseDTO> messages { get; set; }
        }
    }
}