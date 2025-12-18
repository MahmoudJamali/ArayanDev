using Business.Services.Authentication;
using MediatR;
using FluentValidation;




namespace ArayanDev.Business.Handlers.Users.Commands
{

    public class UserLoginCommand : IRequest<(string accessToken, string refreshToken)>
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
        {
            public UserLoginCommandValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("ایمیل الزامی است.")
                    .EmailAddress().WithMessage("ایمیل معتبر نیست.");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("رمز عبور الزامی است.");
            }
        }

        public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, (string accessToken, string refreshToken)>
        {
            private readonly IAuthService _authService;

            public UserLoginCommandHandler(IAuthService authService)
            {
                _authService = authService;
            }

            public async Task<(string accessToken, string refreshToken)> Handle(UserLoginCommand request, CancellationToken cancellationToken)
            {
                return await _authService.LoginWithTokensAsync(request.Email, request.Password);
            }
        }
    }








