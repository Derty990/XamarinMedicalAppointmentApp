using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.WebApi.Dtos
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "Imię jest wymagane.")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format email.")]
        [StringLength(100)]
        public string Email { get; set; }

        public int? AddressId { get; set; } 
    }
}