using System.Net.Http;
using System.Net.Http.Json;
using Business.Services;
using DataAccess.Concrete.Contexts;
using Entities.Concrete;
using Entities.Concrete.Entities.Concrete;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;



public class MelipayamakOtpResponse
{
    public string code { get; set; } = null!;
    public string status { get; set; } = null!;
}

public class OtpService : IOtpService
{
    private readonly HttpClient _client;
    private readonly IConfiguration _config;

    private readonly AppDbContext _context;

    public OtpService(
        HttpClient client,
        IConfiguration config,
        AppDbContext context)
    {
        _client = client;
        _config = config;
        _context = context;

        _client.BaseAddress = new Uri("https://console.melipayamak.com/");
         }


    public async Task SendOtpAsync(string phoneNumber)
    {
        var apiKey = _config["Melipayamak:ApiKey"];

        var response = await _client.PostAsJsonAsync(
            $"api/send/otp/{apiKey}",
            new { to = phoneNumber });

        if (!response.IsSuccessStatusCode)
            throw new Exception("Melipayamak error");

        var result = await response.Content.ReadFromJsonAsync<MelipayamakOtpResponse>();

        if (result == null || string.IsNullOrEmpty(result.code))
            throw new Exception("OTP not returned");

        var user = await _context.Users
    .Include(x => x.Role)
    .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

        if (user == null)
        {
            // گرفتن نقش پیش‌فرض (مثلاً User)
            var defaultRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "User");

            if (defaultRole == null)
                throw new Exception("Default role not found");

            user = new User
            {
                Id = Guid.NewGuid(),
                PhoneNumber = phoneNumber,
                RoleId = defaultRole.Id,
                IsPhoneNumberConfirmed = false,
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }


        // غیرفعال کردن OTPهای قبلی
        var oldOtps = await _context.UserOtps
            .Where(x => x.UserId == user.Id && x.ExpireAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var item in oldOtps)
            item.ExpireAt = DateTime.UtcNow;

        // ذخیره OTP جدید
        var userOtp = new UserOtp
        {
            UserId = user.Id,
            OtpCode = result.code, // بهتره هش بشه
            ExpireAt = DateTime.UtcNow.AddMinutes(3),
            CreatedDate = DateTime.UtcNow,
            AttemptCount = 0
        };

        _context.UserOtps.Add(userOtp);

        await _context.SaveChangesAsync();
    }

    public async Task<OtpVerifyResult> VerifyOtpAsync(string phoneNumber, string otp)
    {
        var now = DateTime.UtcNow;

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

        if (user == null)
            return OtpVerifyResult.InvalidCode;

        // فقط OTP معتبر (منقضی نشده) رو بگیر
        var userOtp = await _context.UserOtps
            .Where(x => x.UserId == user.Id && x.ExpireAt > now)
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefaultAsync();

        if (userOtp == null)
            return OtpVerifyResult.Expired;

        // محدودیت تعداد تلاش
        if (userOtp.AttemptCount >= 5)
            return OtpVerifyResult.TooManyAttempts;

        // بررسی کد
        if (userOtp.OtpCode != otp)
        {
            userOtp.AttemptCount++;
            await _context.SaveChangesAsync();
            return OtpVerifyResult.InvalidCode;
        }

        // ✅ موفق - invalidate کردن OTP
        userOtp.ExpireAt = now;

        user.IsPhoneNumberConfirmed = true;

        await _context.SaveChangesAsync();

        return OtpVerifyResult.Success;
    }



}
