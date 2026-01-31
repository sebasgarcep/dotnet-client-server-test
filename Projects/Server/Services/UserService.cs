using System.Security.Claims;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    class UserService
    {
        private AppDbContext AppDbContext;

        public UserService(AppDbContext appDbContext)
        {
            this.AppDbContext = appDbContext;
        }

        public async Task<User?> GetByClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            var userIdRaw = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdRaw == null)
            {
                return null;
            }

            int userId;
            try
            {
                userId = int.Parse(userIdRaw);
            }
            catch (FormatException)
            {
                return null;
            }

            return await this.GetById(userId);
        }

        public async Task<User?> GetById(int id)
        {
            return await this.AppDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await this.AppDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateUser(User user)
        {
            user = (await this.AppDbContext.Users.AddAsync(user)).Entity;
            await this.AppDbContext.SaveChangesAsync();
            return user;
        }
    }
}