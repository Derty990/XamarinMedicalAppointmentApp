using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.WebApi.ForView
{
    public class DoctorForView
    {
        [Key]
        public int DoctorId { get; set; }
        public int UserId { get; set; } 
        public int SpecializationId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? UserAddressId { get; set; }
        public string SpecializationName { get; set; }

        public static implicit operator DoctorForView(Doctor doctor)
        {
            if (doctor == null) return null;

            var forView = new DoctorForView().CopyProperties(doctor);

            if (doctor.User != null)
            {
                forView.FirstName = doctor.User.FirstName;
                forView.LastName = doctor.User.LastName;
                forView.Email = doctor.User.Email;
                forView.UserAddressId = doctor.User.AddressId;
            }
            if (doctor.Specialization != null)
            {
                forView.SpecializationName = doctor.Specialization.Name;
            }

            return forView;
        }

    }
}