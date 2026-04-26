using DataAccess.Abstract;
using DataAccess.Concrete.Contexts;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly AppDbContext _context;

        public UserProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserProfile
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<bool> ExistsAsync(Guid userId)
        {
            return await _context.UserProfile
                .AnyAsync(x => x.UserId == userId);
        }

        public async Task AddAsync(UserProfile profile)
        {
            await _context.UserProfile.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserProfile profile)
        {
            _context.UserProfile.Update(profile);
            await _context.SaveChangesAsync();
        }
    }
}
