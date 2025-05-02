using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentApp.WebApi.Models
{ 
    [Table("Addresses")]
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }

        [Required]
        [StringLength(150)]
        public string Street { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(10)]
        public string PostalCode { get; set; }

        // --- Właściwości Nawigacyjne ---
        // Jeden adres może być powiązany z wieloma użytkownikami (choć w modelu User.AddressId jest nullable)
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        // Jeden adres może być powiązany z wieloma klinikami
        public virtual ICollection<Clinic> Clinics { get; set; } = new List<Clinic>();
    }
}