using MedicalAppointmentApp.XamarinApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = new ProfileViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as ProfileViewModel)?.OnAppearing();
        }
    }
}