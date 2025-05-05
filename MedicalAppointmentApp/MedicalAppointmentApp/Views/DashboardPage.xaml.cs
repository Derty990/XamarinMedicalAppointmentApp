using MedicalAppointmentApp.XamarinApp.ViewModels; 
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml; 

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : ContentPage
    {
     

        public DashboardPage()
        {
            InitializeComponent();
            BindingContext = new DashboardViewModel();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
           
        }

       
    }
}