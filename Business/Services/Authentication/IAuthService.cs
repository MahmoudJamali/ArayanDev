using Entities.Concrete;

namespace Business.Services.Authentication
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string email, string password);
        Task RegisterAsync(User user, string password);
        Task LogoutAsync();
        Task<string> GenerateJwtTokenAsync(User user);
        Task<(string accessToken, string refreshToken)> LoginWithTokensAsync(string email, string password);
        Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken);

    }
}


