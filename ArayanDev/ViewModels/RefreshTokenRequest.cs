using System.ComponentModel.DataAnnotations;

namespace ArayanDev.Models
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
