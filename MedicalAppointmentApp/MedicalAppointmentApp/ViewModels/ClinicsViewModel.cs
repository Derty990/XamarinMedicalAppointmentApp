using MedicalAppointmentApp.Services.Abstract;
using MedicalAppointmentApp.Views;
using MedicalAppointmentApp.XamarinApp.ApiClient;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class ClinicsViewModel : BaseViewModel
    {
        public string Title { get; }

        private readonly IClinicService _clinicService;
        public ObservableCollection<ClinicForView> Clinics { get; }
        public ICommand LoadClinicsCommand { get; }
        public ICommand AddClinicCommand { get; }
        public ICommand EditClinicCommand { get; }

        private ClinicForView _selectedClinic;
        public ClinicForView SelectedClinic
        {
            get => _selectedClinic;
            set
            {
                if (SetProperty(ref _selectedClinic, value) && value != null)
                {
                    EditClinicCommand.Execute(value);
                }
            }
        }

        public ClinicsViewModel()
        {
            Title = "Kliniki";
            _clinicService = DependencyService.Get<IClinicService>();
            Clinics = new ObservableCollection<ClinicForView>();
            LoadClinicsCommand = new Command(async () => await ExecuteLoadClinicsCommand(), () => !IsBusy);
            AddClinicCommand = new Command(async () => await ExecuteAddClinicCommand(), () => !IsBusy);
            EditClinicCommand = new Command<ClinicForView>(async (clinic) => await ExecuteEditClinicCommand(clinic), (clinic) => !IsBusy && clinic != null);
        }

        async Task ExecuteLoadClinicsCommand()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                Clinics.Clear();
                var items = await _clinicService.GetItemsAsync(true);
                if (items != null)
                {
                    foreach (var item in items) { Clinics.Add(item); }
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Błąd ładowania klinik: {ex.Message}"); }
            finally { IsBusy = false; }
        }

        async Task ExecuteAddClinicCommand()
        {
            if (IsBusy) return;
            await NavigateToEditPage(null);
        }

        async Task ExecuteEditClinicCommand(ClinicForView clinic)
        {
            if (clinic == null || IsBusy) return;
            await NavigateToEditPage(clinic.ClinicId);
        }

        async Task NavigateToEditPage(int? clinicId)
        {
            try
            {
                if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                {
                    await navigationPage.Navigation.PushAsync(new AddEditClinicPage(clinicId));
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Nawigacja do AddEditClinicPage nie powiodła się: {ex.Message}"); }
            finally { SelectedClinic = null; }
        }

        public void OnAppearing()
        {
            IsBusy = false;
            SelectedClinic = null;
            LoadClinicsCommand.Execute(null);
        }

        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsBusy))
            {
                (LoadClinicsCommand as Command)?.ChangeCanExecute();
                (AddClinicCommand as Command)?.ChangeCanExecute();
                (EditClinicCommand as Command)?.ChangeCanExecute();
            }
            else if (propertyName == nameof(SelectedClinic))
            {
                (EditClinicCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}