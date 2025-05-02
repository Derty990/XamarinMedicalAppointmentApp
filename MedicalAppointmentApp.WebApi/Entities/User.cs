using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentApp.WebApi.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        // [EmailAddress] // Można dodać walidację formatu email
        public string Email { get; set; }
        // Uwaga: Ograniczenie UNIQUE dla Email najlepiej skonfigurować w DbContext (OnModelCreating) lub bezpośrednio w bazie

        [Required]
        [StringLength(255)] // Dostosuj długość, jeśli wiesz, jaki będzie hash
        public string PasswordHash { get; set; }

        // Klucz obcy do tabeli Addresses - nullable (dlatego int?)
        public int? AddressId { get; set; }

        [Required]
        public int RoleId { get; set; } // Będzie mapowane na Enum w logice aplikacji

        // --- Właściwości Nawigacyjne ---

        // Właściwość nawigacyjna do powiązanego adresu (może być null)
        // [ForeignKey("AddressId")] wskazuje, która właściwość jest kluczem obcym dla tej nawigacji
        [ForeignKey("AddressId")]
        public virtual Address? Address { get; set; }

        // Właściwość nawigacyjna do powiązanego rekordu Doctor (jeśli ten User jest lekarzem)
        // Relacja jeden-do-jeden (lub zero)
        public virtual Doctor? Doctor { get; set; }

        // Właściwość nawigacyjna dla wizyt, gdzie ten użytkownik jest pacjentem
        [InverseProperty("Patient")] // Pomaga EF Core rozróżnić, która relacja w Appointment dotyczy pacjenta
        public virtual ICollection<Appointment> PatientAppointments { get; set; } = new List<Appointment>();
    }
}