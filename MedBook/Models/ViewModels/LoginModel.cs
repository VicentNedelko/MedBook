using System.ComponentModel.DataAnnotations;

namespace MedBook.Models.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль ")]
        public string Password { get; set; }
    }
}
