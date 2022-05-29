namespace MedBook.Models
{
    public interface IBaseAdmin
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
    }
}
