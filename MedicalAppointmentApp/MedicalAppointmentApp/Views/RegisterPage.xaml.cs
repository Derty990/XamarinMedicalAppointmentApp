using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();

            // Ustaw domyślne wartości
            BirthdayPicker.MaximumDate = DateTime.Today.AddYears(-18);
            GenderPicker.SelectedIndex = 0;
        }

        async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Walidacja podstawowa
            if (string.IsNullOrEmpty(FirstNameEntry.Text) ||
                string.IsNullOrEmpty(LastNameEntry.Text) ||
                string.IsNullOrEmpty(EmailEntry.Text) ||
                string.IsNullOrEmpty(PhoneEntry.Text) ||
                string.IsNullOrEmpty(PasswordEntry.Text))
            {
                await DisplayAlert("Błąd", "Wypełnij wszystkie wymagane pola", "OK");
                return;
            }

            // Walidacja haseł
            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Błąd", "Hasła nie są identyczne", "OK");
                return;
            }

            // Walidacja zgód
            if (!TermsCheckbox.IsChecked || !PrivacyCheckbox.IsChecked)
            {
                await DisplayAlert("Błąd", "Musisz zaakceptować regulamin i wyrazić zgodę na przetwarzanie danych", "OK");
                return;
            }

            try
            {
                // Tutaj w przyszłości byłaby rejestracja w bazie danych

                // Dla celów demonstracyjnych wyświetlamy komunikat sukcesu
                bool result = await DisplayAlert("Sukces",
                    "Konto zostało utworzone pomyślnie! Możesz teraz się zalogować.",
                    "Przejdź do logowania", "OK");

                if (result)
                {
                    // Wróć do strony logowania
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił błąd: {ex.Message}", "OK");
            }
        }

        async void OnLoginTapped(object sender, EventArgs e)
        {
            // Wróć do strony logowania
            await Navigation.PopAsync();
        }
    }
}