namespace MedBook.Models
{
    public class BaseAdmin : User, IBaseAdmin
    {
        public string Login { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string Name { get ; set ; }
    }
}
