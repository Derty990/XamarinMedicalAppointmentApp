using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentApp.WebApi.Entities
{
    public class Clinic
    {
        [Key]
        public int ClinicId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        [EmailAddress] // Dodatkowa walidacja formatu email
        public string? Email { get; set; }

        [Required]
        public bool IsActive { get; set; } = true; // Wartość domyślna ustawiona w C#

        // Navigation property - wizyty w tej klinice
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        // Navigation property - dla relacji wiele-do-wielu z Doctor
        public virtual ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();
    }
}