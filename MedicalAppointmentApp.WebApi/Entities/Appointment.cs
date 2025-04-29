using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentApp.WebApi.Entities
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public int PatientId { get; set; } // Klucz obcy do User (pacjent)

        [Required]
        public int DoctorId { get; set; } // Klucz obcy do Doctor

        [Required]
        public int ClinicId { get; set; } // Klucz obcy do Clinic

        [Required]
        public DateTime AppointmentDate { get; set; } // Połączona data i godzina

        // Usunięto StartTime i EndTime - lepiej używać jednej kolumny DateTime i np. Duration
        // Alternatywnie, jeśli musisz mieć Start i End oddzielnie:
        // public TimeSpan StartTime { get; set; }
        // public TimeSpan EndTime { get; set; }
        // Pamiętaj o CHECK (EndTime > StartTime) - EF Core może wymagać konfiguracji Fluent API dla CHECK

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled"; // Wartość domyślna

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties ---

        [ForeignKey("PatientId")]
        public virtual User Patient { get; set; } = null!;

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;

        [ForeignKey("ClinicId")]
        public virtual Clinic Clinic { get; set; } = null!;

        // Relacja 1-do-wielu z Prescription (jedna wizyta, wiele recept)
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        // Relacja 1-do-1 z MedicalRecord (zakładając jeden wpis na wizytę)
        public virtual MedicalRecord? MedicalRecord { get; set; }
    }
}