using MedicalAppointmentApp.XamarinApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DoctorsPage : ContentPage
    {
        public DoctorsPage()
        {
            InitializeComponent();  
            BindingContext = new DoctorsViewModel();
        }

        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as DoctorsViewModel)?.OnAppearing();
            
        }
    }
}