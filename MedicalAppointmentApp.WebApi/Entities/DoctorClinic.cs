using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentApp.WebApi.Models
{
    [Table("DoctorClinics")]
    public class DoctorClinic
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorClinicId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int ClinicId { get; set; }


        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [ForeignKey("ClinicId")]
        public virtual Clinic Clinic { get; set; }
    }
}