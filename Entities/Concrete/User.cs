using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class User : BaseEntity
    {
        public string Fullname { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;


        public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeExpireAt { get; set; }

        public bool IsPhoneNumberConfirmed { get; set; } = false;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}

