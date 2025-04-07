using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        async void OnLoginClicked(object sender, EventArgs e)
        {
            try
            {
                string email = EmailEntry.Text;
                string password = PasswordEntry.Text;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    await DisplayAlert("Błąd", "Wprowadź email i hasło", "OK");
                    return;
                }

                // Testowe hasło - tylko do celów demonstracyjnych
                if (password == "test123")
                {
                    // Po udanym logowaniu przełączamy się na shell
                    Application.Current.MainPage = new AppShell();
                    await DisplayAlert("Zalogowano pomyślnie", "Zapraszamy", "OK");
                }
                else
                {
                    await DisplayAlert("Błąd", "Niepoprawny email lub hasło", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił błąd: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
            }
        }

        async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Resetowanie hasła", "Funkcja resetowania hasła będzie dostępna wkrótce", "OK");
        }

        void OnRegisterClicked(object sender, EventArgs e)
        {
            
            Application.Current.MainPage = new RegisterPage();

        }
    }
}