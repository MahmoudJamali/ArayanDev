using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using DataAccess.Concrete.Contexts;
using Microsoft.AspNetCore.Http;
using Business.Services;
using Entities.Concrete;
using Entities.Enums;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IOtpService _otpService;

    public AuthService(AppDbContext context, IOtpService otpService)
    {
        _context = context;
        _otpService = otpService;
    }

    public async Task<bool> LoginWithOtpAsync(string phoneNumber, HttpContext http)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

        if (user == null)
            return false;

        var profileCompleted = await _context.UserProfile
            .AnyAsync(x => x.UserId == user.Id);

        string fullName = "";

        if (user.Profile != null && !string.IsNullOrWhiteSpace(user.Profile.Name))
        {
            fullName = $"{user.Profile.Name} {user.Profile.Family}";
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
        new Claim(ClaimTypes.Role, user.Role.Name),
        new Claim("IsPhoneConfirmed", user.IsPhoneNumberConfirmed.ToString()),
        new Claim("ProfileCompleted", profileCompleted ? "true" : "false"),
        new Claim("FullName", fullName)
    };

        var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        var principal = new ClaimsPrincipal(identity);

        var authProps = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTime.UtcNow.AddDays(7)
        };

        await http.SignInAsync("MyCookieAuth", principal, authProps);

        return true;
    }


    public async Task SignInUserAsync(User user, HttpContext http)
    {
        var profileCompleted = user.Profile != null;

        string fullName = "";

        if (user.Profile != null && !string.IsNullOrWhiteSpace(user.Profile.Name))
        {
            fullName = $"{user.Profile.Name} {user.Profile.Family}";
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
        new Claim(ClaimTypes.Role, user.Role.Name),
        new Claim("IsPhoneConfirmed", user.IsPhoneNumberConfirmed.ToString()),
        new Claim("ProfileCompleted", profileCompleted ? "true" : "false"),
        new Claim("FullName", fullName)
    };

        var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        var principal = new ClaimsPrincipal(identity);

        var authProps = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTime.UtcNow.AddDays(7)
        };

        await http.SignInAsync("MyCookieAuth", principal, authProps);
    }



    public async Task<bool> UpdateProfileAsync(Guid userId, UserProfile model)
    {
        // بررسی اینکه آیا پروفایل قبلاً وجود دارد یا نه
        var profile = await _context.UserProfile.FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            // ایجاد پروفایل جدید
            profile = new UserProfile
            {
                UserId = userId,
                Name = model.Name,
                Family = model.Family,
                NationalCode = model.NationalCode,
                City = model.City,
                BirthDay = model.BirthDay,
                EducationDegree = model.EducationDegree,
                Major = model.Major,
                Address = model.Address,
                Email = model.Email
            };
            _context.UserProfile.Add(profile);
        }
        else
        {
            profile.Name = model.Name;
            profile.Family = model.Family;
            profile.NationalCode = model.NationalCode;
            profile.City = model.City;
            profile.BirthDay = model.BirthDay;
            profile.EducationDegree = model.EducationDegree;
            profile.Major = model.Major;
            profile.Address = model.Address;
            profile.Email = model.Email;
        }

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }


    public async Task LogoutAsync(HttpContext http)
    {
        await http.SignOutAsync("MyCookieAuth");
    }
}
