// Plik: Models/AppointmentStatus.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Potrzebne dla ICollection

namespace MedicalAppointmentApp.WebApi.Models // Zmień namespace
{
    [Table("AppointmentStatuses")]
    public class AppointmentStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StatusId { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusName { get; set; } // Np. 'Scheduled', 'Completed', 'Cancelled'

        // Właściwość Nawigacyjna: Jeden status może być przypisany do wielu wizyt
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}