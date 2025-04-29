using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentApp.WebApi.Entities
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }

        [Required]
        public int PatientId { get; set; } // Klucz obcy do User

        [Required]
        public int DoctorId { get; set; } // Klucz obcy do Doctor

        // Klucz obcy do Appointment (może być null, jeśli recepta nie jest z wizyty)
        // Usunięto UNIQUE z bazy, więc jedna wizyta może mieć wiele recept
        public int? AppointmentId { get; set; }

        [Required]
        [MaxLength(500)]
        public required string Medication { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Dosage { get; set; }

        [Required]
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; }

        // --- Navigation Properties ---

        [ForeignKey("PatientId")]
        public virtual User Patient { get; set; } = null!;

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;

        // Wiele recept może być powiązanych z jedną wizytą
        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
    }
}