using MedicalAppointmentApp.XamarinApp.ViewModels; // Using dla ViewModelu
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views // Sprawdź namespace
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditSpecializationPage : ContentPage
    {
        // Przechowujemy ViewModel, aby móc wywołać jego metody
        private AddEditSpecializationViewModel _viewModel;
        private readonly int? _specializationId; // Przechowujemy ID dla OnAppearing

        // Konstruktor dla trybu DODAWANIA (bez ID)
        public AddEditSpecializationPage()
        {
            InitializeComponent();
            _specializationId = null; // Brak ID
            _viewModel = new AddEditSpecializationViewModel(); // Tworzymy VM bez ID
            BindingContext = _viewModel;
            // Tytuł zostanie ustawiony przez ViewModel
        }

        // Konstruktor dla trybu EDYCJI (z ID)
        public AddEditSpecializationPage(int specializationId)
        {
            InitializeComponent();
            _specializationId = specializationId; // Zapisujemy ID
            _viewModel = new AddEditSpecializationViewModel(specializationId); // Tworzymy VM z ID
            BindingContext = _viewModel;
            // Tytuł zostanie ustawiony przez ViewModel
            // Ładowanie danych zrobimy w OnAppearing
        }

        // Metoda cyklu życia strony - ładuje dane tylko w trybie edycji
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Jeśli jesteśmy w trybie edycji (ID zostało przekazane), załaduj dane
            if (_viewModel != null && _viewModel.IsEditMode) // Sprawdź IsEditMode z VM
            {
                await _viewModel.LoadSpecializationAsync(); // Wywołaj ładowanie
            }
        }
    }
}