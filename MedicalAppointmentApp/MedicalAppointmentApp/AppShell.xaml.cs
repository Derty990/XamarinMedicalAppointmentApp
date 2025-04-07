using MedicalAppointmentApp.Views;
using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Rejestracja tras dla stron, które nie są częścią menu flyout
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("register", typeof(RegisterPage));
            Routing.RegisterRoute("doctordetail", typeof(DoctorDetailPage));
        }
    }
}