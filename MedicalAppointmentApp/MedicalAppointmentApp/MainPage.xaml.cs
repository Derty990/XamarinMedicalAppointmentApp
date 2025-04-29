
using MedicalAppointmentApp.Views;
using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    await DisplayAlert("Błąd", "Wprowadź email i hasło", "OK");
                    return;
                }

                // prosta nawigacja zamiast przełączania całej strony głównej
                await Navigation.PushAsync(new DashboardPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił błąd: {ex.Message}", "OK");

                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
            }

        }

        async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}