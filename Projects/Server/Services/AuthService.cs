using Data.Models;
using Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Services
{
    class AuthService
    {
        private AppDbContext AppDbContext;
        private UserService UserService;

        public AuthService(AppDbContext appDbContext, UserService userService)
        {
            this.AppDbContext = appDbContext;
            this.UserService = userService;
        }

        public async Task<User?> SignIn(string email, string password)
        {
            var user = await this.UserService.GetByEmail(email);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }


        public async Task<User?> SignUp(string email, string password)
        {
            var existingUser = await this.UserService.GetByEmail(email);
            if (existingUser != null)
            {
                return null;
            }
            var hash = HashPassword(password);
            var user = new User { Email = email, PasswordHash = hash };
            return await this.UserService.CreateUser(user);
        }

        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32; // 256 bits
        private const int Iterations = 10000;

        private static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            var hashBytes = new byte[SaltSize + KeySize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

            return Convert.ToBase64String(hashBytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            var hashBytes = Convert.FromBase64String(hash);

            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var key = new byte[KeySize];
            Array.Copy(hashBytes, SaltSize, key, 0, KeySize);

            var keyToCheck = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize
                );

            return keyToCheck.SequenceEqual(key);
        }
    }
}
