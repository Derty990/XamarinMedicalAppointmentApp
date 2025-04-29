
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
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Błąd", "Wprowadź email i hasło", "OK");
                return;
            }

            // W tym momencie nie sprawdzamy faktycznie danych - tylko do celów demonstracyjnych
            // W prawdziwej aplikacji byłoby tu uwierzytelnianie

            // Testowe hasło dla demonstracji
            if (password == "test123")
            {
                // Przejście do głównej aplikacji
                Application.Current.MainPage = new FlyoutPage
                {
                    Flyout = new MenuPage(),
                    Detail = new NavigationPage(new DashboardPage())
                };
            }
            else
            {
                await DisplayAlert("Błąd", "Niepoprawny email lub hasło", "OK");
            }
        }

        async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Resetowanie hasła", "Funkcja resetowania hasła będzie dostępna wkrótce", "OK");
        }

        async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}