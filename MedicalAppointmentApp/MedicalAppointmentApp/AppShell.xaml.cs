using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            var dashboardContent = new ShellContent
            {
                Content = new Views.DashboardPage(),
                Title = "Dashboard"
            };

            Items.Add(dashboardContent);
        }
    }
}