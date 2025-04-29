
using System;
using Xamarin.Forms;


namespace MedicalAppointmentApp.Views
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.AddMenuButton();
        }

        async void OnFindDoctorClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new DoctorsPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie można przejść do strony lekarzy: {ex.Message}", "OK");
            }
        }

        async void OnBookAppointmentClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new AppointmentBookingPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie można przejść do strony rezerwacji: {ex.Message}", "OK");
            }
        }

        async void OnMyAppointmentsClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new MyAppointmentsPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie można przejść do strony moich wizyt: {ex.Message}", "OK");
            }
        }

        async void OnProfileClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ProfilePage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie można przejść do strony profilu: {ex.Message}", "OK");
            }
        }
    }
}