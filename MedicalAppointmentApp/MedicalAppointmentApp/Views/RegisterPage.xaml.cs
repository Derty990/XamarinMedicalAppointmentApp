using MedicalAppointmentApp.XamarinApp.ViewModels; 
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics; 

namespace MedicalAppointmentApp.Views 
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            
                BindingContext = new RegisterViewModel();
            
        }

        async void OnLoginTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}