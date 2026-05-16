using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Concrete;
using Entities.Concrete.Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IOtpRepository
    {
        Task<UserOtp?> GetLastOtpByPhoneAsync(string phoneNumber);
    }

}


