using MediatR;
using FluentValidation;
using Business.Services;
using Entities.Enums;

namespace Business.Handlers.Authentication.Commands
{
    // ✅ تغییر نوع خروجی از bool به OtpVerifyResult
    public class VerifyOtpCommand : IRequest<OtpVerifyResult>
    {
        public string PhoneNumber { get; set; } = null!;
        public string OtpCode { get; set; } = null!;
    }

    public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
    {
        public VerifyOtpCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("شماره موبایل الزامی است.");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("کد OTP الزامی است.")
                .Length(6).WithMessage("کد OTP باید ۶ رقمی باشد.");
        }
    }

    // ✅ تغییر نوع Handler
    public class VerifyOtpCommandHandler
        : IRequestHandler<VerifyOtpCommand, OtpVerifyResult>
    {
        private readonly IOtpService _otpService;

        public VerifyOtpCommandHandler(IOtpService otpService)
        {
            _otpService = otpService;
        }

        public async Task<OtpVerifyResult> Handle(
            VerifyOtpCommand request,
            CancellationToken cancellationToken)
        {
            return await _otpService.VerifyOtpAsync(
                request.PhoneNumber,
                request.OtpCode);
        }
    }
}

