using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.WebApi.Dtos
{
    // DTO do tworzenia/aktualizacji kliniki
    public class ClinicCreateDto
    {
        [Required(ErrorMessage = "Nazwa kliniki jest wymagana.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "ID adresu jest wymagane.")]
        public int AddressId { get; set; } // Klient musi podać ID istniejącego adresu
    }
}