using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.UserProfiles.Queries

{
    public class GetUserProfileByUserIdQuery : IRequest<UserProfile>
    {
        public Guid UserId { get; set; }
    }

    public class GetUserProfileByUserIdHandler
        : IRequestHandler<GetUserProfileByUserIdQuery, UserProfile>
    {
        private readonly IUserProfileRepository _repo;

        public GetUserProfileByUserIdHandler(IUserProfileRepository repo)
        {
            _repo = repo;
        }

        public async Task<UserProfile> Handle(GetUserProfileByUserIdQuery request,
                                              CancellationToken cancellationToken)
        {
            return await _repo.GetByUserIdAsync(request.UserId);
        }
    }
}
