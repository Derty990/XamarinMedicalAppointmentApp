using MedicalAppointmentApp.XamarinApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppointmentStatusesPage : ContentPage
    {
        public AppointmentStatusesPage()
        {
            InitializeComponent();
            BindingContext = new AppointmentStatusesViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as AppointmentStatusesViewModel)?.OnAppearing();
        }
    }
}