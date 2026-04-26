using MediatR;

using Entities.Concrete;
using FluentValidation;
using DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.UserProfiles.Commands
{
    public class CompleteProfileCommand
        : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public CompleteProfileModel Model { get; set; } = null!;
    }

    // ---------- ViewModel ----------
    public class CompleteProfileModel
    {
        public string? City { get; set; }
        public int? Age { get; set; }
        public string? EducationDegree { get; set; }
        public string? Major { get; set; }
    }

    // ---------- Validator ----------
    public class CompleteProfileValidator : AbstractValidator<CompleteProfileCommand>
    {
        public CompleteProfileValidator()
        {
            RuleFor(x => x.Model.City)
                .NotEmpty().WithMessage("لطفا شهر را وارد کنید");

            RuleFor(x => x.Model.Age)
                .NotNull().WithMessage("سن را وارد کنید")
                .InclusiveBetween(10, 90)
                .WithMessage("سن معتبر نیست");

            RuleFor(x => x.Model.EducationDegree)
                .NotEmpty().WithMessage("مدرک تحصیلی را وارد کنید");

            RuleFor(x => x.Model.Major)
                .NotEmpty().WithMessage("رشته تحصیلی را وارد کنید");
        }
    }

    // ---------- Handler ----------
    public class CompleteProfileHandler
    : IRequestHandler<CompleteProfileCommand, bool>
    {
        private readonly IUserProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompleteProfileHandler(
            IUserProfileRepository profileRepository,
            IUserRepository userRepository,
            IAuthService authService,
            IHttpContextAccessor httpContextAccessor)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(CompleteProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(request.UserId);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = request.UserId,
                    City = request.Model.City,
                    Age = request.Model.Age,
                    EducationDegree = request.Model.EducationDegree,
                    Major = request.Model.Major
                };

                await _profileRepository.AddAsync(profile);
            }
            else
            {
                profile.City = request.Model.City;
                profile.Age = request.Model.Age;
                profile.EducationDegree = request.Model.EducationDegree;
                profile.Major = request.Model.Major;

                await _profileRepository.UpdateAsync(profile);
            }

            // گرفتن کاربر برای ساخت claims جدید
            var user = await _userRepository.GetByIdWithRoleAndProfileAsync(request.UserId);

            // ساخت مجدد Cookie و Claims
            await _authService.SignInUserAsync(user, _httpContextAccessor.HttpContext!);

            return true;
        }
    }


}
