using MedicalAppointmentApp.XamarinApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClinicsPage : ContentPage
    {
        public ClinicsPage()
        {
            InitializeComponent();
            BindingContext = new ClinicsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as ClinicsViewModel)?.OnAppearing();
        }
    }
}