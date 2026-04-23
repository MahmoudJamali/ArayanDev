using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    namespace Entities.Concrete
    {
        public class UserOtp : BaseEntity
        {
            public Guid UserId { get; set; }
            public User User { get; set; } = null!;

            public string OtpHash { get; set; } = null!;   // store hashed OTP
            public DateTime ExpireAt { get; set; }         // expiration time

            public int AttemptCount { get; set; } = 0;     // wrong attempts

            public string? RequestIp { get; set; }         // optional: for rate-limit
        }
    }

}


