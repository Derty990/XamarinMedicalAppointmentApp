using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace MedicalAppointmentApp.WebApi.Entities
{
    public class Specialization
    {
        [Key] // Oznacza klucz główny
        public int SpecializationId { get; set; }

        [Required] // Odpowiednik NOT NULL
        [MaxLength(100)] // Maksymalna długość dla NVARCHAR(100)
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; } // Znak '?' oznacza, że pole może być NULL

        // Navigation property - kolekcja lekarzy tej specjalizacji
        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}