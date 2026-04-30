using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Enums;

namespace Entities.Concrete
{
    public class CourseEnrollment : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public CourseType CourseType { get; set; } = CourseType.InPerson;
        public DateTime EnrollDate { get; set; } = DateTime.UtcNow;
    }
}



