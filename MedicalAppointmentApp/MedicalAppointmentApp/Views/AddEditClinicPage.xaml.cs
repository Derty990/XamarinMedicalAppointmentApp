using MedicalAppointmentApp.XamarinApp.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditClinicPage : ContentPage
    {
        private readonly int? _clinicId;
        private AddEditClinicViewModel _viewModel; // Referencja do ViewModelu

        // Konstruktor dla trybu DODAWANIA
        public AddEditClinicPage() : this(null) { }

        // Konstruktor dla trybu EDYCJI (i używany przez pierwszy)
        public AddEditClinicPage(int? clinicId)
        {
            InitializeComponent();
            _clinicId = clinicId;
            _viewModel = new AddEditClinicViewModel(_clinicId);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel != null) // Wywołaj inicjalizację ViewModelu
            {
                await _viewModel.InitializeAsync();
            }
        }
    }
}