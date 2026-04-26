using Entities.Concrete;


namespace DataAccess.Abstract
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByUserIdAsync(Guid userId);

        Task AddAsync(UserProfile profile);

        Task UpdateAsync(UserProfile profile);

        Task<bool> ExistsAsync(Guid userId);
    }
}

