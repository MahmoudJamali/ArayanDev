using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Handlers.Users.Commands;
using DataAccess.Concrete.Contexts;
using Entities.Concrete;
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

    public UpdateUserProfileCommandHandler(
        AppDbContext context,
        IAuthService authService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _authService = authService;
        _httpContextAccessor = httpContextAccessor;
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

        profile.Name = request.Model.Name;
        profile.Family = request.Model.Family;
        profile.NationalCode = request.Model.NationalCode;
        profile.City = request.Model.City;
        profile.EducationDegree = request.Model.EducationDegree;
        profile.Major = request.Model.Major;
        profile.Address = request.Model.Address;
        profile.Email = request.Model.Email;

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




