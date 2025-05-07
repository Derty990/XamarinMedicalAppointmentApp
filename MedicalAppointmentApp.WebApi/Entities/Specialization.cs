using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace MedicalAppointmentApp.WebApi.Models
{
    [Table("Specializations")] // Mapuje tę klasę na tabelę "Specializations" w bazie danych
    public class Specialization
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Odpowiada IDENTITY(1,1) w SQL Server
        public int SpecializationId { get; set; }

        [Required]
        [StringLength(100)] 
        public string Name { get; set; }

        // Wskazuje, że jedna specjalizacja może być powiązana z wieloma lekarzami.
        // 'virtual' pozwala na tzw. "lazy loading" w EF Core (ładowanie powiązanych danych na żądanie).
        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}