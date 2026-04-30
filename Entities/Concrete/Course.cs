using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Enums;

namespace Entities.Concrete
{
    public class Course : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public bool IsFree { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;
       
        public ICollection<CourseEnrollment> Enrollments { get; set; }
            = new List<CourseEnrollment>();
    }
}



