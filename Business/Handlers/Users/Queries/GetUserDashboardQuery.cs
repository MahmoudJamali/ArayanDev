using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Handlers.Users.Queries;
using DataAccess.Abstract;
using DataAccess.Concrete.Contexts;
using MediatR;
using DataAccess.Concrete.Contexts;
using Microsoft.EntityFrameworkCore;
using Entities.Enums;
using Microsoft.AspNetCore.Http;


namespace Business.Handlers.Users.Queries
{
    public class UserDashboardViewModel
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }
        public string Family { get; set; }
        public string PhoneNumber { get; set; }

        public string NationalCode { get; set; }
        public string City { get; set; }
        public DateOnly BirthDay { get; set; }
        public string Address { get; set; }
        public string EducationDegree { get; set; }
        public string Major { get; set; }
        public string Email { get; set; }
        public string? ProfileImage { get; set; }


        public List<UserCourseItemViewModel> Courses { get; set; }
            = new();
    }

    public class UserCourseItemViewModel
    {
        public Guid CourseId { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public bool IsFree { get; set; }

        public DateTime? StartDate { get; set; }

        public CourseType CourseType { get; set; }

        public DateTime EnrollDate { get; set; }
    }
}


public class GetUserDashboardQuery : IRequest<UserDashboardViewModel>
{
    public Guid UserId { get; set; }
}




public class GetUserDashboardQueryHandler
    : IRequestHandler<GetUserDashboardQuery, UserDashboardViewModel>
{
    private readonly AppDbContext _context;

    public GetUserDashboardQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDashboardViewModel> Handle(
        GetUserDashboardQuery request,
        CancellationToken cancellationToken)
    {

        var user = await _context.Users
            .Include(x => x.Profile)
            .FirstOrDefaultAsync(x => x.Id == request.UserId);

        if (user == null)
            return null;

        var courses = await _context.CourseEnrollment
            .Where(x => x.UserId == request.UserId)
            .Include(x => x.Course)
            .Select(x => new UserCourseItemViewModel
            {
                CourseId = x.CourseId,
                Title = x.Course.Title,
                Price = x.Course.Price,
                IsFree = x.Course.IsFree,
                StartDate = x.Course.StartDate,
                CourseType = x.CourseType,
                EnrollDate = x.EnrollDate
            })
            .ToListAsync();

        return new UserDashboardViewModel
        {
            UserId = user.Id,
            PhoneNumber = user.PhoneNumber,

            Name = user.Profile?.Name,
            Family = user.Profile?.Family,
            NationalCode = user.Profile?.NationalCode,
            City = user.Profile?.City,
            BirthDay = user.Profile?.BirthDay ?? default,
            EducationDegree = user.Profile?.EducationDegree,
            Major = user.Profile?.Major,
            Email = user.Profile?.Email,
            Address = user.Profile?.Address,
            ProfileImage = user.Profile?.ProfileImage,
            Courses = courses
        };

    }
}





