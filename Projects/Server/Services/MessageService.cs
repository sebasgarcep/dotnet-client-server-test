using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class MessageService
    {
        private readonly AppDbContext AppDbContext;

        public MessageService(AppDbContext context)
        {
            AppDbContext = context;
        }

        public async Task<Message> AddMessage(Message message)
        {
            message = (await AppDbContext.Messages.AddAsync(message)).Entity;
            await AppDbContext.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetAllMessagesOrderedByCreationTime(User user)
        {
            return await AppDbContext.Messages
                .Where(m => m.UserId == user.Id)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}