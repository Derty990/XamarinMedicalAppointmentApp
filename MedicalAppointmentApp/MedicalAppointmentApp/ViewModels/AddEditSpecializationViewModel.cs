using MedicalAppointmentApp.Services.Abstract;
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
    public class AddEditSpecializationViewModel : BaseViewModel
    {
        private readonly ISpecializationService _specializationService;
        private readonly int? _specializationId;

        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        public bool IsEditMode => _specializationId.HasValue;
        public string Title { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand LoadCommand { get; } 

        public AddEditSpecializationViewModel(int? specializationId = null)
        {
            _specializationId = specializationId;
            _specializationService = DependencyService.Get<ISpecializationService>();

            if (_specializationService == null)
            {
                Console.WriteLine("KRYTYCZNY BŁĄD: Nie zarejestrowano ISpecializationService!");

            }

            Title = IsEditMode ? "Edytuj Specjalizację" : "Dodaj Specjalizację";
            SaveCommand = new Command(async () => await ExecuteSaveCommand(), CanExecuteSaveCommand);
            DeleteCommand = new Command(async () => await ExecuteDeleteCommand(), CanExecuteDeleteCommand);
            LoadCommand = new Command(async () => await LoadSpecializationAsync());
            this.PropertyChanged += (_, args) => {
                if (args.PropertyName == nameof(Name))
                {
                    (SaveCommand as Command)?.ChangeCanExecute();
                }
            };
        }
        public async Task LoadSpecializationAsync()
        {
            if (!IsEditMode || !_specializationId.HasValue || IsBusy) return;

            IsBusy = true;
            (SaveCommand as Command)?.ChangeCanExecute();
            (DeleteCommand as Command)?.ChangeCanExecute();
            try
            {
                var spec = await _specializationService.GetItemAsync(_specializationId.Value);
                if (spec != null)
                {
                    Name = spec.Name;
                    Title = $"Edytuj: {Name}"; 
                    OnPropertyChanged(nameof(Title)); 
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Nie znaleziono specjalizacji do edycji.", "OK");
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania specjalizacji: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można załadować danych specjalizacji.", "OK");
            }
            finally
            {
                IsBusy = false;
                (SaveCommand as Command)?.ChangeCanExecute();
                (DeleteCommand as Command)?.ChangeCanExecute();
            }
        }

        private bool CanExecuteSaveCommand()
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(Name);
        }

        async Task ExecuteSaveCommand()
        {
            // Walidacja - bez zmian
            if (!CanExecuteSaveCommand()) // Używamy CanExecute dla spójności
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nazwa specjalizacji nie może być pusta.", "OK");
                return;
            }

            IsBusy = true;
            (SaveCommand as Command)?.ChangeCanExecute();
            (DeleteCommand as Command)?.ChangeCanExecute();

            try
            {
                var specializationData = new SpecializationForView
                {
                    SpecializationId = _specializationId ?? 0,
                    Name = this.Name.Trim()
                };

                if (IsEditMode)
                {
                    await _specializationService.UpdateItemAsync(specializationData);
                    await Application.Current.MainPage.DisplayAlert("Sukces", "Specjalizacja została zaktualizowana.", "OK");
                }
                else
                {
                    await _specializationService.AddItemAsync(specializationData);
                    await Application.Current.MainPage.DisplayAlert("Sukces", "Specjalizacja została dodana.", "OK");
                }
                if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                {
                    await navigationPage.Navigation.PopAsync(); 
                }
                else
                {
                    Debug.WriteLine("Error navigating back after save: MainPage structure incorrect.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd zapisu specjalizacji: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie udało się zapisać specjalizacji. Błąd: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                (SaveCommand as Command)?.ChangeCanExecute();
                (DeleteCommand as Command)?.ChangeCanExecute();
            }
        }

        private bool CanExecuteDeleteCommand()
        {
            return !IsBusy && IsEditMode;
        }

        async Task ExecuteDeleteCommand()
        {
            if (!CanExecuteDeleteCommand()) return;

            bool confirmed = await Application.Current.MainPage.DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usunąć specjalizację '{Name}'?", "Tak, usuń", "Anuluj");
            if (!confirmed) return;

            IsBusy = true;
            (SaveCommand as Command)?.ChangeCanExecute();
            (DeleteCommand as Command)?.ChangeCanExecute();

            try
            {
                await _specializationService.DeleteItemAsync(_specializationId.Value);

                // Jeśli doszliśmy tutaj bez wyjątku - sukces
                await Application.Current.MainPage.DisplayAlert("Sukces", "Specjalizacja została usunięta.", "OK");
                if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                {
                    await navigationPage.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd usuwania specjalizacji: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie udało się usunąć specjalizacji. Błąd: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                (SaveCommand as Command)?.ChangeCanExecute();
                (DeleteCommand as Command)?.ChangeCanExecute();
            }
        }
        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsBusy))
            {
                (SaveCommand as Command)?.ChangeCanExecute();
                (DeleteCommand as Command)?.ChangeCanExecute();
            }
            else if (propertyName == nameof(Name)) 
            {
                (SaveCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}