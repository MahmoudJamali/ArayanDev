using MediatR;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Business.Handlers.Authentication.Commands
{
    public class LoginCommand : IRequest<bool>
    {
        public string PhoneNumber { get; set; } = null!;
        public string Otp { get; set; } = null!;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, bool>
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _http;

        public LoginCommandHandler(IAuthService authService, IHttpContextAccessor http)
        {
            _authService = authService;
            _http = http;
        }

        public async Task<bool> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LoginWithOtpAsync(
                request.PhoneNumber,
                request.Otp,
                _http.HttpContext!
            );
        }
    }

}

