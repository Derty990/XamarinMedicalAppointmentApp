// Używane TYLKO jako parametr wejściowy dla POST /api/users
namespace MedicalAppointmentApp.WebApi.Dtos 
{
    using System.ComponentModel.DataAnnotations;
    public class UserCreateDto
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required][EmailAddress] public string Email { get; set; }
        [Required][MinLength(8)] public string Password { get; set; } // Czyste hasło
        [Required] public int RoleId { get; set; }
        public int? AddressId { get; set; }
    }
}
