using MedicalAppointmentApp.XamarinApp.ViewModels; 
using System;
using System.Threading.Tasks; 
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views 
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditAddressPage : ContentPage
    {
        private readonly int? _addressId; 
        private AddEditAddressViewModel _viewModel; 

    
        public AddEditAddressPage() : this(null)
        {
            // Title ustawi konstruktor ViewModelu
        }

        
        public AddEditAddressPage(int? addressId)
        {
            InitializeComponent();
            _addressId = addressId; 
            _viewModel = new AddEditAddressViewModel(_addressId); 
            BindingContext = _viewModel; 
            
        }

       
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            if (_viewModel != null && _viewModel.IsEditMode)
            {
                
                await _viewModel.InitializeAsync();
                
            }
        }
    }
}