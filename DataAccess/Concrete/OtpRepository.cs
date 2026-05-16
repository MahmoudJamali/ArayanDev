using DataAccess.Abstract;
using DataAccess.Concrete.Contexts;
using Entities.Concrete;
using Entities.Concrete.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly AppDbContext _context;
        public OtpRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserOtp?> GetLastOtpByPhoneAsync(string phoneNumber)
        {
            return await _context.UserOtps
                .Where(x => x.User.PhoneNumber == phoneNumber)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();
        }
    }

}
