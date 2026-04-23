using MediatR;
using FluentValidation;
using DataAccess.Abstract;
using Business.Services;
namespace Business.Handlers.Authentication.Commands
{
    public class SendOtpCommand : IRequest
    {
        public string PhoneNumber { get; set; } = null!;
    }

    public class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
    {
        public SendOtpCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("شماره موبایل الزامی است.")
                .Matches(@"^09\d{9}$").WithMessage("شماره موبایل معتبر نیست.");
        }
    }

    public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand>
    {
        private readonly IOtpService _otpService;

        public SendOtpCommandHandler(IOtpService otpService)
        {
            _otpService = otpService;
        }

        public async Task<Unit> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            await _otpService.SendOtpAsync(request.PhoneNumber);
            return Unit.Value;
        }
    }
}

