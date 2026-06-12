using MediatR;
using Entities.Concrete;
using FluentValidation;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.UserProfiles.Commands
{
    public class CompleteProfileCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public CompleteProfileModel Model { get; set; } = null!;
    }

    // ---------- ViewModel ----------
    public class CompleteProfileModel
    {
        public string Name { get; set; } = null!;
        public string Family { get; set; } = null!;
        public string NationalCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public DateOnly BirthDay { get; set; }
        public string EducationDegree { get; set; } = null!;
        public string Major { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Email { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }

    // ---------- Validator ----------
    public class CompleteProfileValidator : AbstractValidator<CompleteProfileCommand>
    {
        public CompleteProfileValidator()
        {
            RuleFor(x => x.Model.Name)
                .NotEmpty().WithMessage("نام را وارد کنید");

            RuleFor(x => x.Model.Family)
                .NotEmpty().WithMessage("نام خانوادگی را وارد کنید");

            RuleFor(x => x.Model.NationalCode)
                .NotEmpty().WithMessage("کد ملی را وارد کنید")
                .Length(10).WithMessage("کد ملی باید 10 رقم باشد")
                .Matches(@"^\d{10}$").WithMessage("کد ملی معتبر نیست");

            RuleFor(x => x.Model.City)
                .NotEmpty().WithMessage("شهر محل سکونت را وارد کنید");

            RuleFor(x => x.Model.BirthDay)
                .NotEmpty().WithMessage("تاریخ تولد را وارد کنید");

            RuleFor(x => x.Model.Address)
                .NotEmpty().WithMessage("آدرس را وارد کنید");

            RuleFor(x => x.Model.EducationDegree)
                .NotEmpty().WithMessage("مدرک تحصیلی را وارد کنید");

            RuleFor(x => x.Model.Major)
                .NotEmpty().WithMessage("رشته تحصیلی را وارد کنید");

            RuleFor(x => x.Model.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Model.Email))
                .WithMessage("ایمیل معتبر نیست");

            RuleFor(x => x.Model.ProfileImage)
            .Must(file =>
            {
               if (file == null) return true;

                 var allowed = new[] { ".jpg", ".jpeg", ".png" };
               var ext = Path.GetExtension(file.FileName).ToLower();

               return allowed.Contains(ext);
            })
              .WithMessage("فرمت عکس باید jpg یا png باشد");

            RuleFor(x => x.Model.ProfileImage)
            .Must(file =>
            {
                if (file == null) return true;
                return file.Length <= 5 * 1024 * 1024;
            })
            .WithMessage("حجم عکس نباید بیشتر از 5 مگابایت باشد");

        }
    }

    // ---------- Handler ----------
    public class CompleteProfileHandler : IRequestHandler<CompleteProfileCommand, bool>
    {
        private readonly IUserProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageService _imageService;

        public CompleteProfileHandler(
            IUserProfileRepository profileRepository,
            IUserRepository userRepository,
            IAuthService authService, 
            IImageService imageService,
            IHttpContextAccessor httpContextAccessor)
           

        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;

        }

        public async Task<bool> Handle(CompleteProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(request.UserId);

            string? imagePath = null;

            if (request.Model.ProfileImage != null)
            {
                imagePath = await _imageService.SaveProfileImageAsync(request.Model.ProfileImage);
            }


            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = request.UserId,
                    Name = request.Model.Name,
                    Family = request.Model.Family,
                    NationalCode = request.Model.NationalCode,
                    City = request.Model.City,
                    BirthDay = request.Model.BirthDay,
                    EducationDegree = request.Model.EducationDegree,
                    Major = request.Model.Major,
                    Address = request.Model.Address,
                    Email = request.Model.Email,
                    ProfileImage = imagePath
                };

                await _profileRepository.AddAsync(profile);
            }
            else
            {
                profile.Name = request.Model.Name;
                profile.Family = request.Model.Family;
                profile.NationalCode = request.Model.NationalCode;
                profile.City = request.Model.City;
                profile.BirthDay = request.Model.BirthDay;
                profile.EducationDegree = request.Model.EducationDegree;
                profile.Major = request.Model.Major;
                profile.Address = request.Model.Address;
                profile.Email = request.Model.Email;
                if (imagePath != null)
                    profile.ProfileImage = imagePath;


                await _profileRepository.UpdateAsync(profile);
            }

            // گرفتن کاربر برای ساخت Claims جدید
            var user = await _userRepository.GetByIdWithRoleAndProfileAsync(request.UserId);

            // ساخت مجدد Cookie
            await _authService.SignInUserAsync(user, _httpContextAccessor.HttpContext!);

            return true;
        }
    }
}
