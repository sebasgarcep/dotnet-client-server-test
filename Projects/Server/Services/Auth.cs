using Data.Models;
using System.Security.Cryptography;
using Data;

namespace Services
{
    class Auth {
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
                KeySize);

            return keyToCheck.SequenceEqual(key);
        }

        public static User? SignIn(AppDbContext context, string email, string password)
        {
            email = email.ToLower();
            var user = context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        public static User SignUp(AppDbContext context, string email, string password)
        {
            email = email.ToLower();
            var hash = HashPassword(password);
            var user = new User { Email = email, PasswordHash = hash, CreatedAt = DateTime.Now };
            user = context.Users.Add(user).Entity;
            context.SaveChanges();
            return user;
        }
    }
}
