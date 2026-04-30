using Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);

        Task<User?> GetByPhoneAsync(string phoneNumber);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task<bool> ExistsByPhoneAsync(string phoneNumber);
        Task<User> GetByIdWithRoleAndProfileAsync(Guid userId);
    }
}
