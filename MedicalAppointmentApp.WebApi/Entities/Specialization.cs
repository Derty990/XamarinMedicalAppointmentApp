// Plik: Models/Specialization.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Potrzebne dla atrybutu [Table]

namespace MedicalAppointmentApp.WebApi.Models // Zmień na przestrzeń nazw Twojego projektu
{
    [Table("Specializations")] // Mapuje tę klasę na tabelę "Specializations" w bazie danych
    public class Specialization
    {
        [Key] // Oznacza klucz główny
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Odpowiada IDENTITY(1,1) w SQL Server
        public int SpecializationId { get; set; }

        [Required] // Odpowiada NOT NULL w SQL
        [StringLength(100)] // Odpowiada NVARCHAR(100)
        public string Name { get; set; }

        // --- Właściwości Nawigacyjne ---
        // Wskazuje, że jedna specjalizacja może być powiązana z wieloma lekarzami.
        // 'virtual' pozwala na tzw. "lazy loading" w EF Core (ładowanie powiązanych danych na żądanie).
        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}