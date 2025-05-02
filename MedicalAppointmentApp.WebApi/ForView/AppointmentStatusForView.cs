using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.WebApi.ForView
{
    public class AppointmentStatusForView
    {
        [Key]
        public int StatusId { get; set; }
        public string StatusName { get; set; }

        // --- Operatory Konwersji ---
        public static implicit operator AppointmentStatusForView(AppointmentStatus status)
            => status == null ? null : new AppointmentStatusForView().CopyProperties(status);

        // Konwersja w drugą stronę rzadko potrzebna dla statusów, ale możliwa
        public static implicit operator AppointmentStatus(AppointmentStatusForView statusForView)
             => statusForView == null ? null : new AppointmentStatus().CopyProperties(statusForView);
    }
}