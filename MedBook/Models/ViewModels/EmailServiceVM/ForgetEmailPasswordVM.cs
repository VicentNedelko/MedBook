using System.ComponentModel.DataAnnotations;

namespace MedBook.Models.ViewModels.EmailServiceVM
{
    public class ForgetEmailPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
