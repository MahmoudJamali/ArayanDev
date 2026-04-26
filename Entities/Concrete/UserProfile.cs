using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class UserProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string? City { get; set; }
        public int? Age { get; set; }
        public string? EducationDegree { get; set; }
        public string? Major { get; set; }
    }
}



