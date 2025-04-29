using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentApp.WebApi.Entities
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        public int UserId { get; set; } // Klucz obcy do User

        [Required]
        [MaxLength(50)]
        public required string LicenseNumber { get; set; }

        public int? SpecializationId { get; set; } // Klucz obcy do Specialization (może być null)

        [MaxLength(255)]
        public string? PictureUrl { get; set; }

        // --- Navigation Properties ---

        // Relacja 1-do-1 z User
        [ForeignKey("UserId")] // Jawne wskazanie klucza obcego
        public virtual User User { get; set; } = null!; // = null!; oznacza, że oczekujemy, że User zawsze będzie powiązany

        // Relacja 1-do-wielu ze Specialization
        [ForeignKey("SpecializationId")]
        public virtual Specialization? Specialization { get; set; } // Może nie mieć specjalizacji

        // Wizyty prowadzone przez tego lekarza
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        // Recepty wystawione przez tego lekarza
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        // Relacja wiele-do-wielu z Clinic
        public virtual ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();
    }
}