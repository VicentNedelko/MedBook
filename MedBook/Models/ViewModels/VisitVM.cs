using MedBook.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MedBook.Models.ViewModels
{
    public class VisitVM
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("start")]
        public DateTime Start { get; set; }

        [JsonPropertyName("end")]
        public DateTime End { get; set; }

        [JsonIgnore]
        public string Color { get; set; }

        [JsonIgnore]
        public VisitStatus Status { get; set; }

        //public List<Research> Researches { get; set; }

        //public Prescription Prescription { get; set; }

        ////Patient FK
        //public string PatientId { get; set; }
        //public Patient Patient { get; set; }

        ////Doctor FK
        //public string DoctorId { get; set; }
        //public Doctor Doctor { get; set; }
    }
}
