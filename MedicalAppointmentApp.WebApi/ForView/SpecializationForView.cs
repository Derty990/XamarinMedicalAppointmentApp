using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.WebApi.ForView
{
    public class SpecializationForView
    {
        [Key]
        public int SpecializationId { get; set; }
        public string Name { get; set; }

        // --- Operatory Konwersji ---
        public static implicit operator SpecializationForView(Specialization spec)
            => spec == null ? null : new SpecializationForView().CopyProperties(spec);

        public static implicit operator Specialization(SpecializationForView specForView)
             => specForView == null ? null : new Specialization().CopyProperties(specForView);
    }
}