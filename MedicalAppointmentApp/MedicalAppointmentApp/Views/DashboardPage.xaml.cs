using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp.Views
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        void OnFindDoctorClicked(object sender, EventArgs e)
        {
            try
            {
                Shell.Current.GoToAsync("//doctors");
            }
            catch (Exception ex)
            {
                DisplayAlert("Błąd", $"Błąd nawigacji: {ex.Message}", "OK");
            }
        }

        void OnBookAppointmentClicked(object sender, EventArgs e)
        {
            try
            {
                Shell.Current.GoToAsync("//booking");
            }
            catch (Exception ex)
            {
                DisplayAlert("Błąd", $"Błąd nawigacji: {ex.Message}", "OK");
            }
        }

        void OnMyAppointmentsClicked(object sender, EventArgs e)
        {
            try
            {
                Shell.Current.GoToAsync("//appointments");
            }
            catch (Exception ex)
            {
                DisplayAlert("Błąd", $"Błąd nawigacji: {ex.Message}", "OK");
            }
        }

        void OnProfileClicked(object sender, EventArgs e)
        {
            try
            {
                Shell.Current.GoToAsync("//profile");
            }
            catch (Exception ex)
            {
                DisplayAlert("Błąd", $"Błąd nawigacji: {ex.Message}", "OK");
            }
        }
    }
}