// Używane TYLKO jako parametr wejściowy dla PUT /api/users/{id}
namespace MedicalAppointmentApp.WebApi.Dtos
{
    using System.ComponentModel.DataAnnotations;
    public class UserUpdateDto
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public int RoleId { get; set; }
        public int? AddressId { get; set; }
    }
}