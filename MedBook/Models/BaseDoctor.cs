namespace MedBook.Models
{
    public class BaseDoctor : User, IBaseDoctor
    {
        public string Role { get ; set ; }

        public string Password { get ; set ; }
    }
}
