using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.EnrollCourses.Queries
{
    public class EnrollmentListItemDto
    {
        public Guid EnrollmentId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;
        public string? Name { get; set; }
        public string? Family { get; set; }
        public string? City { get; set; }

        public DateTime EnrollDate { get; set; }
    }

    // Query
    public class GetEnrollmentsQuery : IRequest<List<EnrollmentListItemDto>>
    {
        public Guid? CourseId { get; set; } // اگر null بود همه ثبت نام ها
    }

    // Handler
    public class GetEnrollmentsQueryHandler
        : IRequestHandler<GetEnrollmentsQuery, List<EnrollmentListItemDto>>
    {
        private readonly ICourseEnrollmentRepository _enrollmentRepository; // اینترفیس ریپازیتوری

        public GetEnrollmentsQueryHandler(ICourseEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<List<EnrollmentListItemDto>> Handle(
            GetEnrollmentsQuery request,
            CancellationToken cancellationToken)
        {
            // فراخوانی متد جدید ریپازیتوری
            var enrollments = await _enrollmentRepository.GetAllEnrollmentsWithDetailsAsync(request.CourseId);

            // تبدیل لیست CourseEnrollment به لیست EnrollmentListItemDto
            var dtoList = enrollments.Select(e => new EnrollmentListItemDto
            {
                EnrollmentId = e.Id,
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title, // از Course که اینکلود شده

                PhoneNumber = e.User.PhoneNumber, // از User که اینکلود شده
                Name = e.User.Profile != null ? e.User.Profile.Name : null, // از UserProfile که اینکلود شده
                Family = e.User.Profile != null ? e.User.Profile.Family : null,
                City = e.User.Profile != null ? e.User.Profile.City : null,

                EnrollDate = e.EnrollDate
            }).ToList();

            return dtoList;
        }

    }
}


