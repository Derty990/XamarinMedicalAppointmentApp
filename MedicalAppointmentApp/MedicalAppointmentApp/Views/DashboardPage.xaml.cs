using System;
using Xamarin.Forms;
namespace MedicalAppointmentApp.Views
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Initializing DashboardPage");
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("DashboardPage initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing DashboardPage: {ex}");
            }
        }

        void OnFindDoctorClicked(object sender, EventArgs e)
        {
            try
            {
                // Znajdź główny FlyoutPage (AppMainPage)
                var flyoutPage = Application.Current.MainPage as FlyoutPage;
                if (flyoutPage != null)
                {
                    // Nawiguj do DoctorsPage
                    flyoutPage.Detail = new NavigationPage(new DoctorsPage())
                    {
                        BarBackgroundColor = Color.FromHex("#3498db"),
                        BarTextColor = Color.White
                    };

                    // Zamknij menu wysuwane (jeśli było otwarte)
                    flyoutPage.IsPresented = false;
                }
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
                var flyoutPage = Application.Current.MainPage as FlyoutPage;
                if (flyoutPage != null)
                {
                    flyoutPage.Detail = new NavigationPage(new AppointmentBookingPage())
                    {
                        BarBackgroundColor = Color.FromHex("#3498db"),
                        BarTextColor = Color.White
                    };
                    flyoutPage.IsPresented = false;
                }
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
                var flyoutPage = Application.Current.MainPage as FlyoutPage;
                if (flyoutPage != null)
                {
                    flyoutPage.Detail = new NavigationPage(new MyAppointmentsPage())
                    {
                        BarBackgroundColor = Color.FromHex("#3498db"),
                        BarTextColor = Color.White
                    };
                    flyoutPage.IsPresented = false;
                }
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
                var flyoutPage = Application.Current.MainPage as FlyoutPage;
                if (flyoutPage != null)
                {
                    flyoutPage.Detail = new NavigationPage(new ProfilePage())
                    {
                        BarBackgroundColor = Color.FromHex("#3498db"),
                        BarTextColor = Color.White
                    };
                    flyoutPage.IsPresented = false;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Błąd", $"Błąd nawigacji: {ex.Message}", "OK");
            }
        }
    }
}