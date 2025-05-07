using MedicalAppointmentApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        // Property to hold the reference to the main FlyoutPage
        public FlyoutPage ParentFlyoutPage { get; set; }
        public List<FlyoutMenuItem> MenuItems { get; set; }

        public MenuPage()
        {
            InitializeComponent();

            
            MenuItems = new List<FlyoutMenuItem>
            {
                // Add items linking Title to the Page Type you want to navigate to
                new FlyoutMenuItem { Id = 0, Title = "Dashboard", TargetType = typeof(DashboardPage) },
                new FlyoutMenuItem { Id = 1, Title = "Lekarze", TargetType = typeof(DoctorsPage) },
                 new FlyoutMenuItem { Id = 2, Title = "Specjalizacje", TargetType = typeof(SpecializationsPage) },
                 new FlyoutMenuItem { Id = 3, Title = "Adresy", TargetType = typeof(AddressesPage) },
                 new FlyoutMenuItem { Id = 4, Title = "Kliniki", TargetType = typeof(ClinicsPage) },
                 new FlyoutMenuItem { Id = 5, Title = "Mój Profil", TargetType = typeof(ProfilePage) },
                 new FlyoutMenuItem { Id = 9, Title = "Statusy Wizyt", TargetType = typeof(AppointmentStatusesPage) },
                //new FlyoutMenuItem { Id = 2, Title = "Umów wizytę", TargetType = typeof(AppointmentBookingPage) },
                //new FlyoutMenuItem { Id = 3, Title = "Moje wizyty", TargetType = typeof(MyAppointmentsPage) },
                
                
            };

            // Set the ItemsSource for the ListView in XAML
            MenuItemsListView.ItemsSource = MenuItems;

            // Set BindingContext to this page so ItemsSource binding works (simple way)
            // Alternatively, create a MenuViewModel
            BindingContext = this;
        }

        // Event handler for when a menu item is selected
        private void OnMenuItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Get the selected item
            var item = e.SelectedItem as FlyoutMenuItem;
            if (item == null)
                return;

            // Get the Type of the page to navigate to
            Type pageType = item.TargetType;

            // Tell the Parent FlyoutPage to navigate
            if (ParentFlyoutPage != null)
            {
                // Create instance of the target page
                var page = (Page)Activator.CreateInstance(pageType);

                // Set the Detail property of the FlyoutPage
                // Important: Wrap in NavigationPage to enable further navigation stack
                ParentFlyoutPage.Detail = new NavigationPage(page);

                // Hide the Flyout menu
                ParentFlyoutPage.IsPresented = false;
            }

            // De-select the item in the list
            MenuItemsListView.SelectedItem = null;
        }
    }
}