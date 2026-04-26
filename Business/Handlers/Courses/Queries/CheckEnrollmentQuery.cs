using DataAccess.Abstract;
using MediatR;


namespace Business.Handlers.Courses.Queries
{ 
public class CheckEnrollmentQuery : IRequest<bool>
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
}

public class CheckEnrollmentQueryHandler : IRequestHandler<CheckEnrollmentQuery, bool>
{
    private readonly ICourseEnrollmentRepository _repo;

    public CheckEnrollmentQueryHandler(ICourseEnrollmentRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(CheckEnrollmentQuery request, CancellationToken cancellationToken)
    {
        return await _repo.ExistsAsync(request.UserId, request.CourseId);
    }
}

}

