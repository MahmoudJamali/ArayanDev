using DataAccess.Concrete.Contexts;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public interface IAuthService
{
    Task<bool> LoginWithOtpAsync(string phoneNumber,  HttpContext http);
    Task LogoutAsync(HttpContext http);
    Task SignInUserAsync(User user, HttpContext http);
    Task<bool> UpdateProfileAsync(Guid userId, UserProfile model);
}
