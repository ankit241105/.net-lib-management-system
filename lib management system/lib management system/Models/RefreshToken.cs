using System.ComponentModel.DataAnnotations.Schema;

namespace lib_management_system.Models
{
    // A long-lived refresh token, stored server-side so it can be rotated and revoked.
    // The raw value is sent to the browser only inside an httpOnly cookie.
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        [NotMapped]
        public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
    }
}
