using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.WebApi.Dtos
{
    // DTO do aktualizacji specjalizacji lekarza
    public class DoctorUpdateDto
    {
        [Required(ErrorMessage = "ID specjalizacji jest wymagane.")]
        public int SpecializationId { get; set; }
    }
}