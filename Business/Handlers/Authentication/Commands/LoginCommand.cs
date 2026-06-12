using MediatR;
using Microsoft.AspNetCore.Http;
using Business.Services;
using Entities.Enums;

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
        private readonly IHttpContextAccessor _contextAccessor;

        public LoginCommandHandler(
            IAuthService authService,
            IHttpContextAccessor contextAccessor)
        {
            _authService = authService;
            _contextAccessor = contextAccessor;
        }

        public async Task<bool> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // مرحله 1: بررسی OTP
            var verifyResult = await _authService.VerifyOtpAsync(
                request.PhoneNumber,
                request.Otp
            );

            if (verifyResult != OtpVerifyResult.Success)
                return false;

            // مرحله 2: ورود کاربر
            return await _authService.LoginWithOtpAsync(
                request.PhoneNumber,
                _contextAccessor.HttpContext!
            );
        }
    }
}
