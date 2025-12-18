
using DataAccess.Abstract;
using Business.Services.Authentication;
using MediatR;
using FluentValidation;
using Entities.Concrete;
using System.ComponentModel.DataAnnotations;



    namespace ArayanDev.Business.Handlers.Users.Commands
    {
        public class ChangePasswordCommand : IRequest
        {
            public string Email { get; set; } = null!;
            public string OldPassword { get; set; } = null!;
            public string NewPassword { get; set; } = null!;
        [Compare("NewPassword", ErrorMessage = "رمز عبور جدید و تکرار آن یکسان نیستند.")]
        public string ConfirmNewPassword { get; set; } = null!;
    }

    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
        {
            public ChangePasswordCommandValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("ایمیل الزامی است.")
                    .EmailAddress().WithMessage("ایمیل معتبر نیست.");

                RuleFor(x => x.OldPassword)
                    .NotEmpty().WithMessage("رمز فعلی الزامی است.");

                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithMessage("رمز جدید الزامی است.")
                    .MinimumLength(6).WithMessage("رمز عبور باید حداقل ۶ کاراکتر باشد.")
                    .NotEqual(x => x.OldPassword).WithMessage("رمز جدید نباید با رمز قبلی یکسان باشد.");
            }
        }

        public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
        {
            private readonly IUserRepository _userRepository;
            private readonly IPasswordHasher _passwordHasher;

            public ChangePasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
            {
                _userRepository = userRepository;
                _passwordHasher = passwordHasher;
            }

            public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                    throw new Exception("کاربری با این ایمیل یافت نشد.");

                var isValid = _passwordHasher.Verify(request.OldPassword, user.PasswordHash, user.PasswordSalt);
                if (!isValid)
                    throw new Exception("رمز فعلی نادرست است.");

                var (hash, salt) = _passwordHasher.Hash(request.NewPassword);
                user.PasswordHash = hash;
                user.PasswordSalt = salt;

                await _userRepository.UpdateAsync(user);
                return Unit.Value;
            }
        }
    }




