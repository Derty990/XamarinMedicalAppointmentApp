using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MedicalAppointmentApp.Views;
using System;

namespace MedicalAppointmentApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppMainPage : FlyoutPage
    {
        public AppMainPage()
        {
            InitializeComponent();

            // Ustaw stronę menu jako Flyout
            this.Flyout = new MenuPage();

            // Ustaw dashboard jako stronę startową
            this.Detail = new NavigationPage(new DashboardPage())
            {
                BarBackgroundColor = Color.FromHex("#3498db"),
                BarTextColor = Color.White
            };
        }

    }
}