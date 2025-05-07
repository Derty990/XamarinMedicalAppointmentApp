using MedicalAppointmentApp.XamarinApp.ViewModels; // Using dla ViewModelu
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddressesPage : ContentPage
    {
        public AddressesPage()
        {
            InitializeComponent();
            BindingContext = new AddressesViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Wywołaj OnAppearing ViewModelu (które wywoła ładowanie danych)
            (BindingContext as AddressesViewModel)?.OnAppearing();
        }
    }
}