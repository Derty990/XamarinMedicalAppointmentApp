using MedicalAppointmentApp.XamarinApp.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditDoctorPage : ContentPage
    {
        private readonly int _doctorId;
        private EditDoctorViewModel _viewModel;

        // Konstruktor przyjmujący ID lekarza
        public EditDoctorPage(int doctorId)
        {
            InitializeComponent();
            _doctorId = doctorId;
            _viewModel = new EditDoctorViewModel();
            BindingContext = _viewModel;
        }

        // Ładuj dane, gdy strona się pojawia
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Wywołaj ładowanie danych w ViewModelu, przekazując ID
            if (_viewModel != null)
            {
                // Użyj IsBusy z ViewModelu, aby uniknąć wielokrotnego ładowania, jeśli VM to obsługuje
                // lub dodaj flagę _isLoaded do strony, jeśli VM nie ma takiej logiki w LoadDataCommand
                // Poniższy kod wywoła ładowanie za każdym razem - dostosuj wg potrzeb
                await _viewModel.ExecuteLoadDataCommand(_doctorId);
            }
        }
    }
}