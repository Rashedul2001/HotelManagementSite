using System.ComponentModel.DataAnnotations;

namespace HotelManagementSite.Models.Domain
{
    public class VerificationToken
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}