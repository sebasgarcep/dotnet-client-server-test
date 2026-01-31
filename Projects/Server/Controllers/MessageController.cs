using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Services;
using Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace Controllers
{
    public static class MessageController
    {
        public static RouteGroupBuilder MapMessageEndpoints(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder
                .MapPost("/messages", async Task<IResult> (
                    MessageRequestDTO messageRequest,
                    MessageService messageService
                ) => {
                    var message = new Message {
                        Text = messageRequest.Text,
                        User = null,
                    };
                    message = await messageService.AddMessage(message);
                    return Results.Ok(new MessageResponseDTO(message));
                })
                .Produces<MessageResponseDTO>(StatusCodes.Status201Created)
                .RequireAuthorization();

            routeGroupBuilder
                .MapGet("/messages", async Task<IResult> (
                    MessageService messageService
                ) => {
                    var messages = await messageService.GetAllMessagesOrderedByCreationTime(null);
                    var messageDTOs = messages.Select(m => new MessageResponseDTO(m)).ToList();
                    return Results.Ok(new MessageListResponseDTO { messages = messageDTOs });
                })
                .Produces<MessageListResponseDTO>(StatusCodes.Status200OK)
                .RequireAuthorization();

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
            public required List<MessageResponseDTO> messages;
        }
    }
}