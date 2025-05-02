using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentApp.WebApi.Models
{
    [Table("DoctorClinics")]
    public class DoctorClinic
    {
        [Key] // Używamy klucza głównego
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorClinicId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int ClinicId { get; set; }

        // --- Właściwości Nawigacyjne ---

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [ForeignKey("ClinicId")]
        public virtual Clinic Clinic { get; set; }
    }
}