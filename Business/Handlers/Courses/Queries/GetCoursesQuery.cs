using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Courses.Queries
{
    public class GetCoursesQuery : IRequest<List<CourseListItemDto>>
    {
    }
    public class CourseListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }
    }
    public class GetCoursesQueryHandler
    : IRequestHandler<GetCoursesQuery, List<CourseListItemDto>>
    {
        private readonly ICourseRepository _repo;

        public GetCoursesQueryHandler(ICourseRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CourseListItemDto>> Handle(
            GetCoursesQuery request,
            CancellationToken cancellationToken)
        {
            var courses = await _repo.GetAllAsync();

            return courses.Select(c => new CourseListItemDto
            {
                Id = c.Id,
                Title = c.Title,
                Price = c.Price,
                IsFree = c.IsFree
            }).ToList();
        }
    }

}


