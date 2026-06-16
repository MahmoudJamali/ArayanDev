using DataAccess.Concrete.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Users.Queries
{

    public class StudentHomeItemViewModel
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public string? ProfileImage { get; set; }
    }

    public class HomeStudentsViewModel
    {
        public List<StudentHomeItemViewModel> Students { get; set; } = new();
    }

    public class GetHomeStudentsQuery : IRequest<HomeStudentsViewModel>
    {
        public int Take { get; set; } = 8;
    }

    public class GetHomeStudentsQueryHandler
        : IRequestHandler<GetHomeStudentsQuery, HomeStudentsViewModel>
    {
        private readonly AppDbContext _context;

        public GetHomeStudentsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HomeStudentsViewModel> Handle(
            GetHomeStudentsQuery request,
            CancellationToken cancellationToken)
        {

            var students = await _context.CourseEnrollment
                .Include(x => x.User)
                .ThenInclude(x => x.Profile)
                .Where(x => x.User.Profile != null)
                .Select(x => x.User)
                .Distinct()
                .OrderByDescending(x => x.CreatedDate)
                .Take(request.Take)
                .Select(x => new StudentHomeItemViewModel
                {
                    Name = x.Profile.Name,
                    Family = x.Profile.Family,
                    ProfileImage = x.Profile.ProfileImage
                })
                .ToListAsync(cancellationToken);

            return new HomeStudentsViewModel
            {
                Students = students
            };
        }
    }
}
