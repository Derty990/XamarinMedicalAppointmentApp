using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp.Views
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private void NavigateToPage(Page page)
        {
            // Zamknij menu i nawiguj do wybranej strony
            if (Application.Current.MainPage is FlyoutPage flyoutPage)
            {
                flyoutPage.Detail = new NavigationPage(page);
                flyoutPage.IsPresented = false;
            }
        }

        private void OnDashboardClicked(object sender, EventArgs e)
        {
            NavigateToPage(new DashboardPage());
        }

        private void OnDoctorsClicked(object sender, EventArgs e)
        {
            NavigateToPage(new DoctorsPage());
        }

        private void OnAppointmentBookingClicked(object sender, EventArgs e)
        {
            NavigateToPage(new AppointmentBookingPage());
        }

        private void OnMyAppointmentsClicked(object sender, EventArgs e)
        {
            NavigateToPage(new MyAppointmentsPage());
        }

        private void OnProfileClicked(object sender, EventArgs e)
        {
            NavigateToPage(new ProfilePage());
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            // Tutaj logika wylogowania
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}