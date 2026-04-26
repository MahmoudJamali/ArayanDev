using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Abstract;
using DataAccess.Concrete.Contexts;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Course?> GetByIdAsync(Guid id)
        {
            return await _context.Course
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _context.Course
                .Where(c => c.IsActive)
                .ToListAsync();
        }
    }

}


