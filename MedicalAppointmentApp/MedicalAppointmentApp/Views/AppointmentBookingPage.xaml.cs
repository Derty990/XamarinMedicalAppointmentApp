using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp.Views
{
    public partial class AppointmentBookingPage : ContentPage
    {
        public DateTime TodayDate { get; set; }

        public AppointmentBookingPage()
        {
            TodayDate = DateTime.Today;
            BindingContext = this;
            InitializeComponent();

            // Ustawienie domyślnych wartości
            DoctorPicker.SelectedIndex = 0;
            ClinicPicker.SelectedIndex = 0;
            AppointmentDatePicker.Date = DateTime.Today.AddDays(1);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.AddMenuButton();
        }

        async void OnBookAppointmentClicked(object sender, EventArgs e)
        {
            // Walidacja
            if (string.IsNullOrWhiteSpace(ReasonEditor.Text))
            {
                await DisplayAlert("Błąd", "Proszę podać powód wizyty", "OK");
                return;
            }

            // Potwierdzenie rezerwacji
            bool confirmed = await DisplayAlert("Potwierdzenie",
                $"Czy na pewno chcesz zarezerwować wizytę?\n\n" +
                $"Lekarz: {DoctorPicker.SelectedItem}\n" +
                $"Klinika: {ClinicPicker.SelectedItem}\n" +
                $"Data: {AppointmentDatePicker.Date.ToString("dd.MM.yyyy")}\n" +
                $"Godzina: 12:00",
                "Tak", "Anuluj");

            if (confirmed)
            {
                // Symulacja zapisu rezerwacji do bazy
                await DisplayAlert("Sukces", "Wizyta została zarezerwowana pomyślnie!", "OK");

                // Powrót do strony głównej
                await Navigation.PopToRootAsync();
            }
        }
    }
}