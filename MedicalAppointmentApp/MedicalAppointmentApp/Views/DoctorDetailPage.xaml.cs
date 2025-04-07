using MedicalAppointmentApp;
using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp.Views
{
    public partial class DoctorDetailPage : ContentPage
    {
        public DoctorDetailPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Initializing DoctorDetailPage");
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("DoctorDetailPage initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing DoctorDetailPage: {ex}");
            }
        }



        async void OnBookAppointmentClicked(object sender, EventArgs e)
        {
            // Przejście do strony rezerwacji wizyty z wybranym lekarzem
            await Navigation.PushAsync(new AppointmentBookingPage());
        }
    }
}