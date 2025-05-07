using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; 

namespace MedicalAppointmentApp.WebApi.Models
{
    [Table("Doctors")]
    public class Doctor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SpecializationId { get; set; }

        // Usunięto: LicenseNumber, PictureUrl

        // Nawigacja do powiązanego użytkownika
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Nawigacja do specjalizacji
        [ForeignKey("SpecializationId")]
        public virtual Specialization Specialization { get; set; }

        // Nawigacja do wpisów w tabeli łączącej DoctorClinic (kliniki, w których pracuje lekarz)
        public virtual ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();

        // Nawigacja do wizyt prowadzonych przez tego lekarza
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}