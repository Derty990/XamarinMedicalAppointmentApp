using MedicalAppointmentApp.XamarinApp.ViewModels; // Użyj właściwego namespace
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics; // Dla Debug.WriteLine

namespace MedicalAppointmentApp.Views // Użyj właściwego namespace
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent(); // Najpierw inicjalizacja XAML

            // Dodajemy try-catch wokół tworzenia ViewModelu
            try
            {
                BindingContext = new RegisterViewModel(); // Ta linia prawdopodobnie powoduje błąd
            }
            catch (Exception ex) // Łapiemy jakikolwiek wyjątek
            {
                // Wypisz PEŁNE informacje o błędzie do okna Output -> Debug
                Debug.WriteLine($"!!! KRYTYCZNY BŁĄD podczas tworzenia RegisterViewModel: {ex.ToString()}");

                // Pokaż prosty komunikat użytkownikowi
                // Używamy Device.BeginInvokeOnMainThread, bo jesteśmy w konstruktorze
                Device.BeginInvokeOnMainThread(async () => {
                    await DisplayAlert("Błąd Inicjalizacji Strony", $"Nie można załadować strony rejestracji. Błąd: {ex.Message}", "OK");
                    // Można dodać nawigację wstecz, żeby aplikacja całkiem nie utknęła
                    // await Navigation.PopAsync();
                });
            }
        }

        // Metoda nawigacji "Zaloguj się" zostaje bez zmian
        async void OnLoginTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}