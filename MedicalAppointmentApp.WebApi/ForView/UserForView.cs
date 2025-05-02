using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.Helpers; 

namespace MedicalAppointmentApp.WebApi.ForView
{
    public class UserForView
    {
        // Właściwości, które chcemy wysyłać/odbierać przez API
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int? AddressId { get; set; } // Tylko ID adresu
        // BRAK PasswordHash!

        // Operator konwersji Z Encji DO ForView (do wysyłania danych do Xamarin)
        public static implicit operator UserForView(User user)
            => user == null ? null : new UserForView().CopyProperties(user);

        // Operator konwersji Z ForView DO Encji (do odbierania danych - używaj ostrożnie!)
        // Przydatny, jeśli np. aktualizujesz Usera na podstawie UserForView,
        // ale pamiętaj, że nie skopiuje PasswordHash!
        public static implicit operator User(UserForView userForView)
            => userForView == null ? null : new User().CopyProperties(userForView);
    }
}