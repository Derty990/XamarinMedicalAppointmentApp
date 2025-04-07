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

            MainPage = new LoginPage();

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