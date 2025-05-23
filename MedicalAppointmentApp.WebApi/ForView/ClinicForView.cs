﻿using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.WebApi.ForView
{
    public class ClinicForView
    {
        [Key]
        public int ClinicId { get; set; }
        public string Name { get; set; }

        // ID adresu może być przydatne
        public int AddressId { get; set; }

        // Spłaszczone dane adresu
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public static implicit operator ClinicForView(Clinic clinic)
        {
            if (clinic == null) return null;

            var forView = new ClinicForView().CopyProperties(clinic);

            // Ręcznie skopiuj dane z zagnieżdżonego adresu
            // WAŻNE: To zadziała tylko, jeśli w kontrolerze załadowano powiązany Address przez .Include()!
            if (clinic.Address != null)
            {
                forView.Street = clinic.Address.Street;
                forView.City = clinic.Address.City;
                forView.PostalCode = clinic.Address.PostalCode;
            }

            return forView;
        }
    }
}