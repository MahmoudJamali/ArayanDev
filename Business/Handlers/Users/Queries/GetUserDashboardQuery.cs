using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Abstract;

using MediatR;

namespace Business.Handlers.Users.Queries;
public class UserDashboardViewModel
{
    public Guid UserId { get; set; }

    public string PhoneNumber { get; set; }

    public string? Name { get; set; }

    public string? Family { get; set; }

    public string? NationalCode { get; set; }

    public string? City { get; set; }

    public DateOnly BirthDay { get; set; }

    public string? EducationDegree { get; set; }

    public string? Major { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }
}
public class GetUserDashboardQuery : IRequest<UserDashboardViewModel>
{
    public Guid UserId { get; set; }
}

public class GetUserDashboardQueryHandler
    : IRequestHandler<GetUserDashboardQuery, UserDashboardViewModel>
{
    private readonly IUserRepository _userRepository;

    public GetUserDashboardQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDashboardViewModel> Handle(
        GetUserDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithProfile(request.UserId);

        if (user == null)
            return null;

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
            Address = user.Profile?.Address,
            Email = user.Profile?.Email
        };
    }
}



