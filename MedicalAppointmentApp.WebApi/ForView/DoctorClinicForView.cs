using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.WebApi.ForView
{
    public class DoctorClinicForView
    {
        [Key] 
        public int DoctorClinicId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int ClinicId { get; set; }

        public static implicit operator DoctorClinicForView(DoctorClinic dc)
            => dc == null ? null : new DoctorClinicForView().CopyProperties(dc);

        public static implicit operator DoctorClinic(DoctorClinicForView dcForView)
            => dcForView == null ? null : new DoctorClinic().CopyProperties(dcForView);
    }
}