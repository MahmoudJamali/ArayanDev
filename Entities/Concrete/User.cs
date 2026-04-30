using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace Entities.Concrete
    {
        public class User : BaseEntity
        {

            public string PhoneNumber { get; set; } = null!; // Required

            public bool IsActive { get; set; } = true;
            public bool IsPhoneNumberConfirmed { get; set; } = false;

            public Guid RoleId { get; set; }
            public Role Role { get; set; } = null!;
        public UserProfile? Profile { get; set; }
        public ICollection<CourseEnrollment> Enrollments { get; set; }
    = new List<CourseEnrollment>();
        public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();

        }

    }



