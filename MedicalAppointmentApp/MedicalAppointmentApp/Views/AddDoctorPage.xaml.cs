using MedicalAppointmentApp.XamarinApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddDoctorPage : ContentPage
    {
        public AddDoctorPage()
        {
            InitializeComponent();
          
            BindingContext = new AddDoctorViewModel();
        }
      
        protected override void OnAppearing()
        {
            base.OnAppearing();
           
        }
    }
}