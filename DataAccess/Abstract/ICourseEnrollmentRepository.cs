using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface ICourseEnrollmentRepository
    {
        Task<bool> ExistsAsync(Guid userId, Guid courseId);
        Task AddAsync(CourseEnrollment entity);
        Task<List<CourseEnrollment>> GetUserEnrollmentsAsync(Guid userId);
    }
}



