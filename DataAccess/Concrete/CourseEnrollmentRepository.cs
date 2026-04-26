
using Microsoft.EntityFrameworkCore;
using DataAccess.Abstract;
using DataAccess.Concrete.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete
{
    public class CourseEnrollmentRepository : ICourseEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public CourseEnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid courseId)
        {
            return await _context.CourseEnrollment
                .AnyAsync(x => x.UserId == userId && x.CourseId == courseId);
        }

        public async Task AddAsync(CourseEnrollment entity)
        {
            await _context.CourseEnrollment.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CourseEnrollment>> GetUserEnrollmentsAsync(Guid userId)
        {
            return await _context.CourseEnrollment
                .Include(x => x.Course)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }
    }
}




