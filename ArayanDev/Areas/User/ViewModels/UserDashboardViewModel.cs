using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Enums;

namespace UI.Areas.User.ViewModels;

    public class UserDashboardViewModel
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }
        public string Family { get; set; }
        public string PhoneNumber { get; set; }

        public string NationalCode { get; set; }
        public string City { get; set; }
        public DateOnly BirthDay { get; set; }
    public string Address { get; set; }
    public string EducationDegree { get; set; }
        public string Major { get; set; }
        public string Email { get; set; }

        public List<UserCourseItemViewModel> Courses { get; set; }
            = new();
    }

    public class UserCourseItemViewModel
    {
        public Guid CourseId { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public bool IsFree { get; set; }

        public DateTime? StartDate { get; set; }

        public CourseType CourseType { get; set; }

        public DateTime EnrollDate { get; set; }
    }




