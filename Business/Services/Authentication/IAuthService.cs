using DataAccess.Concrete.Contexts;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public interface IAuthService
{
    Task<bool> LoginWithOtpAsync(string phoneNumber, string otp, HttpContext http);
    Task LogoutAsync(HttpContext http);
}
