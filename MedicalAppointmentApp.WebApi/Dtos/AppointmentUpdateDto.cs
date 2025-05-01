using System;
using System.ComponentModel.DataAnnotations;
namespace MedicalAppointmentApp.WebApi.Dtos
{
    public class AppointmentUpdateDto
    {
        [Required] public int StatusId { get; set; }
        [Required] public DateTime AppointmentDate { get; set; }
        [Required] public TimeSpan StartTime { get; set; }
        [Required] public TimeSpan EndTime { get; set; }
    }
}
