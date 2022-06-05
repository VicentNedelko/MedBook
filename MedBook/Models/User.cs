using Microsoft.AspNetCore.Identity;

namespace MedBook.Models
{
    public class User : IdentityUser
    {
        public string? EmailConfirmationToken { get; set; }

        public bool IsBlock { get; set; }
    }
}