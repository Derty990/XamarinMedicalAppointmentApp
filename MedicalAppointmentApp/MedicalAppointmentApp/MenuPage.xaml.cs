using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MedicalAppointmentApp.Views;

namespace MedicalAppointmentApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
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

        private void NavigateToPage(Page page)
        {
            var flyoutPage = (FlyoutPage)Parent;

            if (flyoutPage != null)
            {
                flyoutPage.Detail = new NavigationPage(page)
                {
                    BarBackgroundColor = Color.FromHex("#3498db"),
                    BarTextColor = Color.White
                };

                flyoutPage.IsPresented = false;
            }
        }
    }
}