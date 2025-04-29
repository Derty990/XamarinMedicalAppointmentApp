using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace MedicalAppointmentApp.WebApi.Entities
{
    // Rozważ dodanie tutaj enuma dla RoleId, jeśli chcesz go mieć też w backendzie
    // public enum UserRole { Patient = 1, Doctor = 2, Admin = 3 }

    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public required string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public required string Email { get; set; } // UNIQUE jest obsługiwane przez EF Core na podstawie konwencji lub konfiguracji

        [Required]
        [MaxLength(255)] // Długość zależy od użytego algorytmu hashowania
        public required string PasswordHash { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [Required]
        public int RoleId { get; set; } // Będzie odpowiadać wartościom z Twojego Enuma w aplikacji Xamarin

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime RegisteredDate { get; set; } = DateTime.UtcNow; // Użyj UtcNow dla spójności

        // --- Navigation Properties ---

        // Jeśli User i Doctor to relacja 1-do-1 (jeden User może być jednym Doktorem)
        public virtual Doctor? Doctor { get; set; }

        // Wizyty, gdzie ten użytkownik jest pacjentem
        [InverseProperty("Patient")] // Pomaga EF Core rozróżnić relacje
        public virtual ICollection<Appointment> PatientAppointments { get; set; } = new List<Appointment>();

        // Recepty wystawione dla tego użytkownika (pacjenta)
        [InverseProperty("Patient")]
        public virtual ICollection<Prescription> PatientPrescriptions { get; set; } = new List<Prescription>();

        // Historia medyczna tego użytkownika (pacjenta)
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}