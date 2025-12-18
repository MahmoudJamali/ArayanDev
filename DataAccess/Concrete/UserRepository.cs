using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Abstract.Context;
using DataAccess.Concrete.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.Contexts;
using Entities.Concrete;
namespace DataAccess.Concrete
{
   public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Set<User>()
                .Include(u => u.Role)     // 👈 این خط اضافه شد
                .Include(u => u.Claims)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Set<User>()
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));
        }
        public async Task UpdateAsync(User user)
        {
            _context.Set<User>().Update(user);
            await _context.SaveChangesAsync();
        }
    }
}


