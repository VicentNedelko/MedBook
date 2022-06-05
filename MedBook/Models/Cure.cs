namespace MedBook.Models
{
    public class Cure
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Prescription FK
        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
    }
}
