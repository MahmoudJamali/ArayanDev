
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


        public async Task<List<CourseEnrollment>> GetAllEnrollmentsWithDetailsAsync(Guid? courseId = null)
        {
            var query = _context.CourseEnrollment
                .AsNoTracking() // چون فقط میخوان بخونن، NoTracking بهتره
                .Include(e => e.User)
                    .ThenInclude(u => u.Profile) // اینجا پروفایل یوزر رو هم میاریم
                .Include(e => e.Course)
                .AsQueryable();

            if (courseId.HasValue)
            {
                query = query.Where(e => e.CourseId == courseId.Value);
            }

            // برای نمایش در داشبورد ادمین، معمولا مرتب سازی بر اساس تاریخ ثبت نام مهمه
            return await query
                .OrderByDescending(e => e.EnrollDate)
                .ToListAsync();
        }
    }
}




