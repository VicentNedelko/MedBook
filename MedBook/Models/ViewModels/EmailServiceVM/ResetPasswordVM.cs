using System.ComponentModel.DataAnnotations;

namespace MedBook.Models.ViewModels.EmailServiceVM
{
    public class ResetPasswordVM
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        [StringLength(25, ErrorMessage = "Пароль должен содержать не менее 6 симоволов", MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }

        public string Code { get; set; }
    }
}
