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

        void OnRegisterClicked(object sender, EventArgs e)
        {
            // Bez nawigacji, po prostu sprawdź dane
            if (string.IsNullOrEmpty(FirstNameEntry.Text) ||
                string.IsNullOrEmpty(LastNameEntry.Text) ||
                string.IsNullOrEmpty(EmailEntry.Text) ||
                string.IsNullOrEmpty(PhoneEntry.Text) ||
                string.IsNullOrEmpty(PasswordEntry.Text))
            {
                DisplayAlert("Błąd", "Wypełnij wszystkie wymagane pola", "OK");
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                DisplayAlert("Błąd", "Hasła nie są identyczne", "OK");
                return;
            }

            if (!TermsCheckbox.IsChecked || !PrivacyCheckbox.IsChecked)
            {
                DisplayAlert("Błąd", "Musisz zaakceptować regulamin i wyrazić zgodę na przetwarzanie danych", "OK");
                return;
            }

            // Po pomyślnej rejestracji wróć do LoginPage
            DisplayAlert("Sukces", "Konto zostało utworzone pomyślnie! Możesz teraz się zalogować.", "OK");
            Application.Current.MainPage = new LoginPage();
        }

        void OnLoginTapped(object sender, EventArgs e)
        {
            // Powrót do strony logowania
            Application.Current.MainPage = new LoginPage();
        }

        // Wspólna metoda do powrotu do ekranu logowania
        private void GoBackToLogin()
        {
            Application.Current.MainPage = new LoginPage();
        }
    }
}