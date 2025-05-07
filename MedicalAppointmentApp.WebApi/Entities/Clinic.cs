using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; 

namespace MedicalAppointmentApp.WebApi.Models 
{
    [Table("Clinics")]
    public class Clinic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClinicId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required] // Zakładamy, że klinika musi mieć adres
        public int AddressId { get; set; }

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }

        public virtual ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}