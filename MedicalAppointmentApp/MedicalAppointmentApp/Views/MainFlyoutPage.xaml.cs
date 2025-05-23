﻿using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    // --- CHANGE ContentPage TO FlyoutPage ---
    public partial class MainFlyoutPage : FlyoutPage
    // --------------------------------------
    {
        public MainFlyoutPage()
        {
            InitializeComponent(); 

            // Create the menu page instance
            var menuPage = new MenuPage();

            // Assign the menu page to the Flyout property
            Flyout = menuPage;

            // Set the initial Detail page (e.g., Dashboard)
            // Wrap it in a NavigationPage to allow navigation within the detail section
            Detail = new NavigationPage(new DashboardPage());

            // Give the MenuPage a reference back to this FlyoutPage
            // so it can change the Detail page and hide itself
            menuPage.ParentFlyoutPage = this;

            
        }

       
    }
}