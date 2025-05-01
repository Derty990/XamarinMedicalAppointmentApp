using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.WebApi.Dtos
{
    // DTO do tworzenia rekordu Doctor (powiązania z User i Specialization)
    public class DoctorCreateDto
    {
        [Required(ErrorMessage = "ID użytkownika (lekarza) jest wymagane.")]
        public int UserId { get; set; } // Klient podaje ID istniejącego Usera

        [Required(ErrorMessage = "ID specjalizacji jest wymagane.")]
        public int SpecializationId { get; set; } // Klient podaje ID istniejącej specjalizacji
    }
}