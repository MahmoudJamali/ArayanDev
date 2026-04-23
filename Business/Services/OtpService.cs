using DataAccess.Concrete.Contexts;
using Entities.Concrete;
using Entities.Concrete.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Business.Services
{
    public class OtpService : IOtpService
    {
        private readonly AppDbContext _context;

        public OtpService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendOtpAsync(string phoneNumber)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {
                user = new User
                {
                    PhoneNumber = phoneNumber,
                    RoleId = await _context.Roles
                        .Where(r => r.Name == "User")
                        .Select(r => r.Id)
                        .FirstAsync()
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // OTP 6 digits
            string otp = GenerateOtp();

            var otpEntity = new UserOtp
            {
                UserId = user.Id,
                OtpCode = otp,
                ExpireAt = DateTime.UtcNow.AddMinutes(2),
 
            };

            _context.UserOtps.Add(otpEntity);
            await _context.SaveChangesAsync();

            // به جای SMS واقعی
            Console.WriteLine($"OTP for {phoneNumber}: {otp}");
        }


        public async Task<bool> VerifyOtpAsync(string phoneNumber, string otp)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
                return false;

            var otpEntity = await _context.UserOtps
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.CreatedDate)
                .FirstOrDefaultAsync();

            if (otpEntity == null)
                return false;

            if (otpEntity.ExpireAt < DateTime.UtcNow)
                return false;

            if (otpEntity.AttemptCount >= 5)
                return false;

            if (otpEntity.OtpCode != otp)
            {
                otpEntity.AttemptCount++;
                await _context.SaveChangesAsync();
                return false;
            }

            user.IsPhoneNumberConfirmed = true;
            await _context.SaveChangesAsync();

            return true;
        }


        private string GenerateOtp()
        {
            // استفاده از RNG امن
            int code = RandomNumberGenerator.GetInt32(100000, 999999);
            return code.ToString();
        }
    }
}
