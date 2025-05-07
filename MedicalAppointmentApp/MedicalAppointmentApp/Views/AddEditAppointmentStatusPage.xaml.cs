using MedicalAppointmentApp.XamarinApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditAppointmentStatusPage : ContentPage
    {
        private readonly int? _statusId;
        private AddEditAppointmentStatusViewModel _viewModel;

        public AddEditAppointmentStatusPage(int? statusId = null) // Konstruktor domyślny i dla edycji
        {
            InitializeComponent();
            _statusId = statusId;
            _viewModel = new AddEditAppointmentStatusViewModel(_statusId);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel != null && _viewModel.IsEditMode)
            {
                await _viewModel.InitializeAsync(); // Wywołaj metodę ładowania z VM
            }
        }
    }
}