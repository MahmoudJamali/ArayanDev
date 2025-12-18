using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Abstract;
using DataAccess.Abstract.Context;
using DataAccess.Concrete.Contexts;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;



namespace Business.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly AppDbContext _context;


        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IOptions<JwtSettings> jwtOptions,
            AppDbContext context,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtOptions.Value;
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            var isValid = _passwordHasher.Verify(password, user.PasswordHash, user.PasswordSalt);
            return isValid ? user : null;
        }

        public async Task RegisterAsync(User user, string password)
        {
            var (hash, salt) = _passwordHasher.Hash(password);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public Task LogoutAsync()
        {
            return Task.CompletedTask;
        }

        public Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Fullname),
            new Claim("fullname", user.Fullname),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

            claims.AddRange(user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                signingCredentials: creds
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
        public async Task<(string accessToken, string refreshToken)> LoginWithTokensAsync(string email, string password)
        {
            var user = await LoginAsync(email, password);
            if (user == null)
                throw new UnauthorizedAccessException();

            var accessToken = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            // روش امن: افزودن مستقیم به DbSet
            refreshToken.UserId = user.Id;
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return (accessToken, refreshToken.Token);
        }

        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null)
                throw new UnauthorizedAccessException();

            var token = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);
            if (token == null)
                throw new UnauthorizedAccessException();

            token.IsRevoked = true;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);

            var newAccessToken = await GenerateJwtTokenAsync(user);
            await _userRepository.SaveChangesAsync();

            return (newAccessToken, newRefreshToken.Token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }


    }


}


