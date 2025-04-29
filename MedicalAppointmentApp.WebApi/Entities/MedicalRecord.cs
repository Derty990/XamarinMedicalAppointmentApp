using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentApp.WebApi.Entities
{
    public class MedicalRecord
    {
        [Key]
        public int RecordId { get; set; }

        [Required]
        public int PatientId { get; set; } // Klucz obcy do User

        // Klucz obcy do Appointment (może być null, jeśli rekord nie jest powiązany z konkretną wizytą?)
        // UNIQUE jest implikowane przez relację 1-do-1 z Appointment poniżej
        public int? AppointmentId { get; set; }

        [MaxLength(500)]
        public string? Diagnosis { get; set; }

        [Required]
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties ---

        [ForeignKey("PatientId")]
        public virtual User Patient { get; set; } = null!;

        // Relacja 1-do-1 (lub 1-do-0..1) z Appointment
        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
    }
}