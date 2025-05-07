using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.Helpers;
using System.ComponentModel.DataAnnotations; 

namespace MedicalAppointmentApp.WebApi.ForView
{
    public class AddressForView 
    {
        [Key] 
        public int AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        // --- Operatory Konwersji ---
        public static implicit operator AddressForView(Address address)
            => address == null ? null : new AddressForView().CopyProperties(address);

        public static implicit operator Address(AddressForView addressForView)
            => addressForView == null ? null : new Address().CopyProperties(addressForView);
    }
}