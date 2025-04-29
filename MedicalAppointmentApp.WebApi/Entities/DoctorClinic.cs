using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentApp.WebApi.Entities
{
    // Ta klasa reprezentuje tabelę łączącą DoctorClinics
    // EF Core potrafi wykryć relację wiele-do-wielu i skonfigurować klucz złożony
    public class DoctorClinic
    {
        // Można dodać własny klucz główny lub polegać na kluczu złożonym (DoctorId, ClinicId)
        // Dodanie własnego klucza może uprościć niektóre operacje CRUD
        [Key]
        public int DoctorClinicId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int ClinicId { get; set; }

        // --- Navigation Properties ---
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;

        [ForeignKey("ClinicId")]
        public virtual Clinic Clinic { get; set; } = null!;

        // Można tu dodać dodatkowe pola specyficzne dla tej relacji, np.
        // public string? DoctorRoomNumber { get; set; }
    }
}