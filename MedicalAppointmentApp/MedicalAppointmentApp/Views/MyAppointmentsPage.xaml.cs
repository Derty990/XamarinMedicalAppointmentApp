using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp.Views
{
    public partial class MyAppointmentsPage : ContentPage
    {
        public MyAppointmentsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.AddMenuButton();
        }

        void OnUpcomingTabClicked(object sender, EventArgs e)
        {
            UpcomingButton.Style = (Style)Resources["ActiveTabButtonStyle"];
            HistoryButton.Style = (Style)Resources["TabButtonStyle"];

            // Tu w przyszłości: Załaduj nadchodzące wizyty
        }

        void OnHistoryTabClicked(object sender, EventArgs e)
        {
            UpcomingButton.Style = (Style)Resources["TabButtonStyle"];
            HistoryButton.Style = (Style)Resources["ActiveTabButtonStyle"];

            // Tu w przyszłości: Załaduj historię wizyt
        }

        async void OnCancelAppointmentClicked(object sender, EventArgs e)
        {
            bool confirmed = await DisplayAlert("Anulowanie wizyty",
                "Czy na pewno chcesz anulować tę wizytę?",
                "Tak", "Nie");

            if (confirmed)
            {
                await DisplayAlert("Sukces", "Wizyta została anulowana", "OK");

                // Tu w przyszłości: Aktualizacja danych w bazie
                // i odświeżenie listy wizyt
            }
        }

        async void OnRescheduleAppointmentClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Zmiana terminu",
                "Funkcja zmiany terminu będzie dostępna wkrótce",
                "OK");

            // Tu w przyszłości: Przejście do strony zmiany terminu
        }
    }
}