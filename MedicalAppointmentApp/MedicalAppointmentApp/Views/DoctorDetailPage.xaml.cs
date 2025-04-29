using MedicalAppointmentApp.Views;
using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp.Views
{
    public partial class DoctorDetailPage : ContentPage
    {
        public DoctorDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.AddMenuButton();
        }

        async void OnBookAppointmentClicked(object sender, EventArgs e)
        {
            // Przejście do strony rezerwacji wizyty z wybranym lekarzem
            await Navigation.PushAsync(new AppointmentBookingPage());
        }
    }
}