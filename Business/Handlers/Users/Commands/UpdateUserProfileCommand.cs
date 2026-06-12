using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Handlers.UserProfiles.Commands;
using Business.Handlers.Users.Commands;
using DataAccess.Concrete.Contexts;
using Entities.Concrete;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace Business.Handlers.Users.Commands
{
    public class EditProfileViewModel
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }
        public string Family { get; set; }
        public string NationalCode { get; set; }
        public string City { get; set; }

        public int? BirthYear { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDayNumber { get; set; }

        public string EducationDegree { get; set; }
        public string Major { get; set; }
        public string Address { get; set; }
        public string? Email { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }
}


public class UpdateUserProfileCommand : IRequest<bool>
{
    public EditProfileViewModel Model { get; set; }
    public DateOnly? BirthDay { get; set; }
}

public class UpdateUserProfileCommandHandler
    : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IImageService _imageService;
    public UpdateUserProfileCommandHandler(
        AppDbContext context,
        IAuthService authService,
        IImageService imageService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _authService = authService;
        _httpContextAccessor = httpContextAccessor;
        _imageService = imageService;
    }
    // ---------- Validator ----------
    public class UpdateProfileValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateProfileValidator()
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
    public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _context.UserProfile
            .FirstOrDefaultAsync(x => x.UserId == request.Model.UserId);

        if (profile == null)
        {
            profile = new UserProfile
            {
                UserId = request.Model.UserId
            };

            _context.UserProfile.Add(profile);
        }
        string? imagePath = null;

        if (request.Model.ProfileImage != null)
        {
            imagePath = await _imageService.SaveProfileImageAsync(request.Model.ProfileImage);
        }

        profile.Name = request.Model.Name;
        profile.Family = request.Model.Family;
        profile.NationalCode = request.Model.NationalCode;
        profile.City = request.Model.City;
        profile.EducationDegree = request.Model.EducationDegree;
        profile.Major = request.Model.Major;
        profile.Address = request.Model.Address;
        profile.Email = request.Model.Email;
        if (imagePath != null)
            profile.ProfileImage = imagePath;
        if (request.BirthDay.HasValue)
            profile.BirthDay = request.BirthDay.Value;

        await _context.SaveChangesAsync();

        // گرفتن کاربر برای ساخت Claims جدید
        var user = await _context.Users
            .Include(x => x.Role)
            .Include(x => x.Profile)
            .FirstOrDefaultAsync(x => x.Id == request.Model.UserId);

        if (user != null)
        {
            await _authService.SignInUserAsync(user, _httpContextAccessor.HttpContext!);
        }

        return true;
    }
}




