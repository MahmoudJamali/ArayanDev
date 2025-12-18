using System.ComponentModel.DataAnnotations;

namespace ArayanDev.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "ایمیل الزامی است.")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر نیست.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}