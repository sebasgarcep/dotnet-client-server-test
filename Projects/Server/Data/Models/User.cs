using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User : IAuditableEntity
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}