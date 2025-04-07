using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Initializing LoginPage");
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("LoginPage initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing LoginPage: {ex}");
            }
            
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

                if (password == "test123")
                {
                    try
                    {
                        var mainPage = new AppMainPage();

                        Application.Current.MainPage = mainPage;

                        await DisplayAlert("Zalogowano pomyślnie", "Zapraszamy", "OK");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Błąd", $"Nie udało się utworzyć interfejsu: {ex.Message}", "OK");
                        System.Diagnostics.Debug.WriteLine($"Error: {ex}");
                    }
                }
                else
                {
                    await DisplayAlert("Błąd", "Niepoprawny email lub hasło", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił błąd: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Exception in login: {ex}");
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