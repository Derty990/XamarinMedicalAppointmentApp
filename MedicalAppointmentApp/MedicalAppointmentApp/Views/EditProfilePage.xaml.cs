using MedicalAppointmentApp.XamarinApp.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicalAppointmentApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProfilePage : ContentPage
    {
        private readonly int _userId;
        private EditProfileViewModel _viewModel;

        // Konstruktor przyjmujący ID użytkownika
        public EditProfilePage(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _viewModel = new EditProfileViewModel(_userId);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel != null)
            {
                // Wywołaj metodę inicjalizującą ViewModelu (która załaduje dane)
                await _viewModel.InitializeAsync();
            }
        }
    }
}