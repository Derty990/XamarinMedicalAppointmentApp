// Plik: Models/Appointment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentApp.WebApi.Models // Zmień namespace
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentId { get; set; }

        [Required]
        public int PatientId { get; set; } // Klucz obcy do Users (Pacjent)

        [Required]
        public int DoctorId { get; set; } // Klucz obcy do Doctors

        [Required]
        public int ClinicId { get; set; } // Klucz obcy do Clinics

        [Required]
        [Column(TypeName = "date")] // Mapowanie na typ DATE w SQL
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Column(TypeName = "time")] // Mapowanie na typ TIME w SQL
        public TimeSpan StartTime { get; set; } // TimeSpan dobrze mapuje się na TIME

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan EndTime { get; set; }

        [Required]
        public int StatusId { get; set; } // Klucz obcy do AppointmentStatuses

        // Usunięto: Notes, CreatedDate

        // --- Właściwości Nawigacyjne ---

        [ForeignKey("PatientId")]
        public virtual User Patient { get; set; } // Jawne nazwanie dla jasności

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [ForeignKey("ClinicId")]
        public virtual Clinic Clinic { get; set; }

        [ForeignKey("StatusId")]
        public virtual AppointmentStatus Status { get; set; }
    }
}