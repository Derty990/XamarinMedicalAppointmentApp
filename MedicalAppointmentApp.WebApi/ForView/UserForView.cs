using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.Helpers; 

namespace MedicalAppointmentApp.WebApi.ForView
{
    public class UserForView
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int? AddressId { get; set; } 

      
        public static implicit operator UserForView(User user)
            => user == null ? null : new UserForView().CopyProperties(user);

        public static implicit operator User(UserForView userForView)
            => userForView == null ? null : new User().CopyProperties(userForView);
    }
}