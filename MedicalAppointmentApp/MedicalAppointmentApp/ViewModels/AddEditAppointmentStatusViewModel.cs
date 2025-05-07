using MedicalAppointmentApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient;    
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using MedicalAppointmentApp.XamarinApp.Services.Abstract;

namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class AddEditAppointmentStatusViewModel : BaseViewModel
    {
        private readonly IAppointmentStatusService _statusService;
        private readonly int? _statusId;

        private string _statusName;
        public string StatusName { get => _statusName; set => SetProperty(ref _statusName, value); }
        public string Title { get; private set; }

        public bool IsEditMode => _statusId.HasValue;

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public AddEditAppointmentStatusViewModel(int? statusId = null)
        {
            _statusId = statusId;
            _statusService = DependencyService.Get<IAppointmentStatusService>();
            if (_statusService == null) { Console.WriteLine("KRYTYCZNY BŁĄD: Nie zarejestrowano IAppointmentStatusService!"); }

            Title = IsEditMode ? "Edytuj Status Wizyty" : "Dodaj Status Wizyty";

            SaveCommand = new Command(async () => await ExecuteSaveCommand(), CanExecuteSaveCommand);
            DeleteCommand = new Command(async () => await ExecuteDeleteCommand(), CanExecuteDeleteCommand);

            PropertyChanged += (_, args) => {
                if (args.PropertyName == nameof(StatusName) || args.PropertyName == nameof(IsBusy))
                    NotifyCommandCanExecuteChanged();
            };
        }

        public async Task InitializeAsync()
        {
            if (IsEditMode && _statusId.HasValue)
            {
                IsBusy = true;
                NotifyCommandCanExecuteChanged();
                try
                {
                    var status = await _statusService.GetItemAsync(_statusId.Value);
                    if (status != null)
                    {
                        StatusName = status.StatusName;
                        Title = $"Edytuj: {StatusName}";
                        OnPropertyChanged(nameof(Title));
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Błąd", "Nie znaleziono statusu.", "OK");
                        await PopPageAsync();
                    }
                }
                catch (Exception ex) { Debug.WriteLine($"Błąd ładowania statusu: {ex.Message}"); }
                finally { IsBusy = false; NotifyCommandCanExecuteChanged(); }
            }
        }

        private bool CanExecuteSaveCommand() => !IsBusy && !string.IsNullOrWhiteSpace(StatusName);
        private bool CanExecuteDeleteCommand() => !IsBusy && IsEditMode;

        private void NotifyCommandCanExecuteChanged()
        {
            (SaveCommand as Command)?.ChangeCanExecute();
            (DeleteCommand as Command)?.ChangeCanExecute();
        }
        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsBusy) || propertyName == nameof(StatusName))
            {
                NotifyCommandCanExecuteChanged();
            }
        }

        async Task ExecuteSaveCommand()
        {
            if (!CanExecuteSaveCommand()) { await Application.Current.MainPage.DisplayAlert("Błąd", "Nazwa statusu jest wymagana.", "OK"); return; }
            IsBusy = true; NotifyCommandCanExecuteChanged();
            try
            {
                var statusData = new AppointmentStatusForView
                {
                    StatusId = _statusId ?? 0,
                    StatusName = this.StatusName.Trim()
                };

                if (IsEditMode)
                {
                    await _statusService.UpdateItemAsync(statusData);
                }
                else
                {
                    await _statusService.AddItemAsync(statusData);
                }
                await Application.Current.MainPage.DisplayAlert("Sukces", $"Status {(IsEditMode ? "zaktualizowany" : "dodany")}.", "OK");
                await PopPageAsync();
            }
            catch (Exception ex) { Debug.WriteLine($"Błąd zapisu statusu: {ex.Message}");  }
            finally { IsBusy = false; NotifyCommandCanExecuteChanged(); }
        }

        async Task ExecuteDeleteCommand()
        {
            if (!CanExecuteDeleteCommand()) return;
            bool confirmed = await Application.Current.MainPage.DisplayAlert("Potwierdzenie", $"Usunąć status '{StatusName}'?", "Tak", "Nie");
            if (!confirmed) return;
            IsBusy = true; NotifyCommandCanExecuteChanged();
            try
            {
                await _statusService.DeleteItemAsync(_statusId.Value);
                await Application.Current.MainPage.DisplayAlert("Sukces", "Status został usunięty.", "OK");
                await PopPageAsync();
            }
            catch (Exception ex) { Debug.WriteLine($"Błąd usuwania statusu: {ex.Message}");  }
            finally { IsBusy = false; NotifyCommandCanExecuteChanged(); }
        }
        private async Task PopPageAsync() { if (Application.Current.MainPage is FlyoutPage fp && fp.Detail is NavigationPage np) await np.Navigation.PopAsync(); }
    }
}