using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.WebApi.Dtos
{
    // DTO do tworzenia (lub aktualizacji) adresu
    public class AddressCreateDto
    {
        [Required(ErrorMessage = "Ulica jest wymagana.")]
        [StringLength(150)]
        public string Street { get; set; }

        [Required(ErrorMessage = "Miasto jest wymagane.")]
        [StringLength(100)]
        public string City { get; set; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany.")]
        [StringLength(10)]
        public string PostalCode { get; set; }
    }
}