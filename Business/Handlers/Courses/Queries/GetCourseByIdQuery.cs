using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Courses.Queries
{
    // -------------------------
    // QUERY
    // -------------------------
    public class GetCourseByIdQuery : IRequest<Course>
    {
        public Guid CourseId { get; set; }
    }

    // -------------------------
    // HANDLER
    // -------------------------
    public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, Course>
    {
        private readonly ICourseRepository _courseRepository;

        public GetCourseByIdQueryHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<Course> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            // گرفتن دوره از دیتابیس
            var course = await _courseRepository.GetByIdAsync(request.CourseId);

            return course; // اگر نبود null برمی‌گردد و کنترلر NotFound می‌دهد
        }
    }
}



