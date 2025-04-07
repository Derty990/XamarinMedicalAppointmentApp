using MedicalAppointmentApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Add this global exception handler
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            try
            {
                InitializeComponent();
                MainPage = new LoginPage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in App constructor: {ex}");
            }

            /*MainPage = new AppShell();

            bool isUserLoggedIn = false;

            *if (isUserLoggedIn)
            {
                MainPage = new AppShell();
            }
            else
            {
                
                MainPage = new LoginPage();

                
            }*/


        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine($"UNHANDLED EXCEPTION: {ex?.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex?.StackTrace}");
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}