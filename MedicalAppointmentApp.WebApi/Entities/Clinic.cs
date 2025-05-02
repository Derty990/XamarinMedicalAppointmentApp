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

        // Klucz obcy do tabeli Addresses
        [Required] // Zakładamy, że klinika musi mieć adres
        public int AddressId { get; set; }

        // Usunięto: Phone, Email, IsActive

        // --- Właściwości Nawigacyjne ---

        // Nawigacja do powiązanego adresu
        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }

        // Nawigacja do wpisów w tabeli łączącej DoctorClinic (lekarze pracujący w tej klinice)
        public virtual ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();

        // Nawigacja do wizyt odbywających się w tej klinice
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}