using System;
using System.ComponentModel.DataAnnotations;
namespace MedicalAppointmentApp.WebApi.Dtos
{
    public class AppointmentCreateDto
    {
        [Required] public int PatientId { get; set; }
        [Required] public int DoctorId { get; set; }
        [Required] public int ClinicId { get; set; }
        [Required] public int StatusId { get; set; }
        [Required] public DateTime AppointmentDate { get; set; }
        [Required] public TimeSpan StartTime { get; set; }
        [Required] public TimeSpan EndTime { get; set; }
    }
}