using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Areas.User.ViewModels
{
    public class EditProfileViewModel
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }
        public string Family { get; set; }
        public string NationalCode { get; set; }
        public string City { get; set; }

        public int? BirthYear { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDayNumber { get; set; }

        public string EducationDegree { get; set; }
        public string Major { get; set; }
        public string Address { get; set; }
        public string? Email { get; set; }
    }
}




