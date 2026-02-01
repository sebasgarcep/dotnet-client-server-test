using Services;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Controllers
{
    public static class MessageController
    {
        public static RouteGroupBuilder MapMessageEndpoints(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapGet("/", GetMessages);
            routeGroupBuilder.MapPost("/", PostMessage);
            return routeGroupBuilder;
        }

        [Authorize]
        private static async Task<Results<Ok<MessageListResponseDTO>, UnauthorizedHttpResult>> GetMessages(
            ClaimsPrincipal claimsPrincipal,
            MessageService messageService,
            UserService userService
        )
        {
            var user = await userService.GetByClaimsPrincipal(claimsPrincipal);
            if (user == null)
            {
                return TypedResults.Unauthorized();
            }

            var messages = await messageService.GetAllMessagesOrderedByCreationTime(user);
            var messageDTOs = messages.Select(m => new MessageResponseDTO(m)).ToList();
            return TypedResults.Ok(new MessageListResponseDTO { messages = messageDTOs });
        }

        [Authorize]
        private static async Task<Results<Ok<MessageResponseDTO>, UnauthorizedHttpResult>> PostMessage(
            ClaimsPrincipal claimsPrincipal,
            MessageRequestDTO messageRequest,
            UserService userService,
            MessageService messageService
        )
        {
            var user = await userService.GetByClaimsPrincipal(claimsPrincipal);
            if (user == null)
            {
                return TypedResults.Unauthorized();
            }

            var message = new Message
            {
                Text = messageRequest.Text,
                User = user,
            };
            message = await messageService.AddMessage(message);
            return TypedResults.Ok(new MessageResponseDTO(message));
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