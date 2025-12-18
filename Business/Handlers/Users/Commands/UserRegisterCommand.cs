
using DataAccess.Abstract;
using Business.Services.Authentication;
using MediatR;
using FluentValidation;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using DataAccess.Concrete.Contexts;

namespace ArayanDev.Business.Handlers.Users.Commands
{
    public class UserRegisterCommand : IRequest<(string accessToken, string refreshToken)>
    {
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = null!;

    }
    public class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
    {
        public UserRegisterCommandValidator()
        {
            RuleFor(x => x.Fullname)
                .NotEmpty().WithMessage("نام کامل الزامی است.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("ایمیل الزامی است.")
                .EmailAddress().WithMessage("ایمیل معتبر نیست.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?\d{10,15}$")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("شماره موبایل معتبر نیست.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("رمز عبور الزامی است.")
                .MinimumLength(6).WithMessage("رمز عبور باید حداقل ۶ کاراکتر باشد.");
        }
    }

    // 3. Handler
    public class Handler : IRequestHandler<UserRegisterCommand, (string accessToken, string refreshToken)>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;


        public Handler(IUserRepository userRepository, IAuthService authService, AppDbContext context)
        {
            _userRepository = userRepository;
            _authService = authService;
            _context = context;
        }

        public async Task<(string accessToken, string refreshToken)> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            var userRole = await _context.Roles.FirstAsync(r => r.Name == "User");
            var user = new User
            {
                Fullname = request.Fullname,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleId = userRole.Id
            };

            await _authService.RegisterAsync(user, request.Password);
            return await _authService.LoginWithTokensAsync(user.Email, request.Password);
        }
    }
}





