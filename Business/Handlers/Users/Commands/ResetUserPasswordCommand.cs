using Business.Services.Authentication;
using MediatR;
using FluentValidation;
using DataAccess.Abstract;




namespace ArayanDev.Business.Handlers.Users.Commands
{
    public class ResetUserPasswordCommand : IRequest
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
    {
        public ResetUserPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("ایمیل الزامی است.")
                .EmailAddress().WithMessage("ایمیل معتبر نیست.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("کد بازیابی الزامی است.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("رمز جدید الزامی است.")
                .MinimumLength(6).WithMessage("رمز عبور باید حداقل ۶ کاراکتر باشد.");
        }
    }

    public class ResetUserPasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher) : IRequestHandler<ResetUserPasswordCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<Unit> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("کاربری با این ایمیل یافت نشد.");

            if (user.VerificationCode != request.Code || user.VerificationCodeExpireAt < DateTime.UtcNow)
                throw new Exception("کد بازیابی نامعتبر یا منقضی شده است.");

            var (hash, salt) = _passwordHasher.Hash(request.NewPassword);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.VerificationCode = null;
            user.VerificationCodeExpireAt = null;

            await _userRepository.UpdateAsync(user);
            return Unit.Value;
        }
    }
}






