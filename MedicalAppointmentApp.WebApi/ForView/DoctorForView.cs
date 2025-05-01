using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.WebApi.ForView
{
    public class DoctorForView
    {
        [Key]
        public int DoctorId { get; set; }
        public int UserId { get; set; } // ID powiązanego użytkownika
        public int SpecializationId { get; set; }

        // Spłaszczone dane z powiązanego User
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? UserAddressId { get; set; } // Opcjonalne ID adresu lekarza

        // Spłaszczone dane z powiązanej Specialization
        public string SpecializationName { get; set; }

        // --- Operator Konwersji z Encji Doctor ---
        // Wymaga ręcznego mapowania spłaszczonych pól!
        public static implicit operator DoctorForView(Doctor doctor)
        {
            if (doctor == null) return null;

            // Skopiuj pasujące pola (DoctorId, UserId, SpecializationId)
            var forView = new DoctorForView().CopyProperties(doctor);

            // Ręcznie wypełnij dane z User i Specialization
            // WAŻNE: Wymaga .Include(d => d.User) i .Include(d => d.Specialization) w kontrolerze!
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

        // Konwersja DoctorForView -> Doctor nie jest zazwyczaj potrzebna,
        // bo rekord Doctor tworzy się podając UserId i SpecializationId (używając np. DoctorCreateDto)
        // a aktualizuje się np. tylko SpecializationId (używając DoctorUpdateDto).
    }
}