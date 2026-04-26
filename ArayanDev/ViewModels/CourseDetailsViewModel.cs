using System.ComponentModel.DataAnnotations;
using Entities.Concrete;

namespace ArayanDev.Models
{
    public class CourseDetailsViewModel
    {
        public Course Course { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool IsEnrolled { get; set; }
        public bool HasProfile { get; set; }
    }

}
