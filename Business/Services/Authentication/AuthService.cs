using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using DataAccess.Concrete.Contexts;
using Microsoft.AspNetCore.Http;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IOtpService _otpService;

    public AuthService(AppDbContext context, IOtpService otpService)
    {
        _context = context;
        _otpService = otpService;
    }

    public async Task<bool> LoginWithOtpAsync(string phoneNumber, string otp, HttpContext http)
    {
        // ابتدا OTP را چک کنیم
        var isValid = await _otpService.VerifyOtpAsync(phoneNumber, otp);
        if (!isValid)
            return false;

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

        if (user == null)
            return false;

        // Claim ها
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("IsPhoneConfirmed", user.IsPhoneNumberConfirmed.ToString())
        };

        var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        var principal = new ClaimsPrincipal(identity);

        var authProps = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTime.UtcNow.AddDays(7)
        };

        // ساخت کوکی ورود
        await http.SignInAsync("MyCookieAuth", principal, authProps);

        return true;
    }

    public async Task LogoutAsync(HttpContext http)
    {
        await http.SignOutAsync("MyCookieAuth");
    }
}
