using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.WebApi.Dtos
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress] //walidacja
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
