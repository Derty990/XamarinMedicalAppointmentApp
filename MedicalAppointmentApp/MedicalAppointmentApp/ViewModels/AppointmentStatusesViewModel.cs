using MedicalAppointmentApp.Services.Abstract;
using MedicalAppointmentApp.Views;
using MedicalAppointmentApp.XamarinApp.ApiClient;
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class AppointmentStatusesViewModel : BaseViewModel
    {
        public string Title { get; }

        private readonly IAppointmentStatusService _statusService;

        public ObservableCollection<AppointmentStatusForView> Statuses { get; }

        public ICommand LoadStatusesCommand { get; }
        public ICommand AddStatusCommand { get; }
        public ICommand EditStatusCommand { get; }

        private AppointmentStatusForView _selectedStatus;
        public AppointmentStatusForView SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (SetProperty(ref _selectedStatus, value) && value != null)
                {
                    EditStatusCommand.Execute(value);
                }
            }
        }

        public AppointmentStatusesViewModel()
        {
            Title = "Statusy Wizyt";
            _statusService = DependencyService.Get<IAppointmentStatusService>();
            if (_statusService == null) { Console.WriteLine("KRYTYCZNY BŁĄD: Nie zarejestrowano IAppointmentStatusService!"); }

            Statuses = new ObservableCollection<AppointmentStatusForView>();

            LoadStatusesCommand = new Command(async () => await ExecuteLoadStatusesCommand(), () => !IsBusy);
            AddStatusCommand = new Command(async () => await ExecuteAddStatusCommand(), () => !IsBusy);
            EditStatusCommand = new Command<AppointmentStatusForView>(async (status) => await ExecuteEditStatusCommand(status), (status) => !IsBusy && status != null);
        }

        async Task ExecuteLoadStatusesCommand()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                Statuses.Clear();
                var items = await _statusService.GetItemsAsync(true); 
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Statuses.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania statusów wizyt: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można załadować listy statusów wizyt.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteAddStatusCommand()
        {
            if (IsBusy) return;
            await NavigateToEditPage(null); 
        }

        async Task ExecuteEditStatusCommand(AppointmentStatusForView status)
        {
            if (status == null || IsBusy) return;
            await NavigateToEditPage(status.StatusId);
        }

        async Task NavigateToEditPage(int? statusId)
        {
            try
            {
                if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                {
                    await navigationPage.Navigation.PushAsync(new AddEditAppointmentStatusPage(statusId));
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Nawigacja do AddEditAppointmentStatusPage nie powiodła się: {ex.Message}"); }
            finally
            {
                SelectedStatus = null; 
            }
        }

        public void OnAppearing()
        {
            IsBusy = false; 
            SelectedStatus = null; 
            LoadStatusesCommand.Execute(null); 
        }

        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsBusy))
            {
                (LoadStatusesCommand as Command)?.ChangeCanExecute();
                (AddStatusCommand as Command)?.ChangeCanExecute();
                (EditStatusCommand as Command)?.ChangeCanExecute();
            }
            else if (propertyName == nameof(SelectedStatus))
            {
                (EditStatusCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}