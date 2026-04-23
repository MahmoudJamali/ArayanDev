using MediatR;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Business.Handlers.Authentication.Commands
{
    public class LoginCommand : IRequest
    {
        public string PhoneNumber { get; set; } = null!;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _http;

        public LoginCommandHandler(IUserRepository userRepository, IHttpContextAccessor http)
        {
            _userRepository = userRepository;
            _http = http;
        }

        public async Task<Unit> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByPhoneAsync(request.PhoneNumber);

            if (user == null)
                throw new Exception("کاربر یافت نشد.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _http.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(12)
                });

            return Unit.Value;
        }
    }
}

