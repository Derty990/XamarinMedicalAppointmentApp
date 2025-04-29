using MedicalAppointmentApp.Views;
using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp.Views
{
    public partial class DoctorsPage : ContentPage
    {
        public DoctorsPage()
        {
            InitializeComponent();

            // Przykładowe dane - w przyszłości będą pobierane z bazy
            var doctors = new[]
            {
                new { Name = "Dr Anna Kowalska", Specialization = "Kardiolog", Clinic = "Klinika Zdrowia", Rating = "⭐⭐⭐⭐⭐ (15 opinii)" },
                new { Name = "Dr Jan Nowak", Specialization = "Okulista", Clinic = "Centrum Medyczne", Rating = "⭐⭐⭐⭐ (8 opinii)" },
                new { Name = "Dr Piotr Wiśniewski", Specialization = "Neurolog", Clinic = "Klinika Zdrowia", Rating = "⭐⭐⭐⭐⭐ (23 opinie)" },
                new { Name = "Dr Maria Dąbrowska", Specialization = "Dermatolog", Clinic = "Przychodnia Specjalistyczna", Rating = "⭐⭐⭐⭐ (12 opinii)" },
                new { Name = "Dr Tomasz Lewandowski", Specialization = "Pediatra", Clinic = "Centrum Pediatryczne", Rating = "⭐⭐⭐⭐⭐ (31 opinii)" }
            };

            DoctorsListView.ItemsSource = doctors;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.AddMenuButton();
        }

        async void OnDoctorSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            // Przekazanie danych wybranego lekarza do strony szczegółów
            await Navigation.PushAsync(new DoctorDetailPage());

            // Odznaczenie wybranego elementu
            ((ListView)sender).SelectedItem = null;
        }
    }
}