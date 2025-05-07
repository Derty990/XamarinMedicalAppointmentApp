using MedicalAppointmentApp.Services.Abstract;     
using MedicalAppointmentApp.XamarinApp.ApiClient; 
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class AddEditClinicViewModel : BaseViewModel
    {
        private readonly IClinicService _clinicService;
        private readonly IAddressService _addressService;
        private readonly int? _clinicId; // Null dla dodawania, ID dla edycji

        // Właściwości formularza
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ObservableCollection<AddressForView> AvailableAddresses { get; }

        private AddressForView _selectedAddress;
        public AddressForView SelectedAddress
        {
            get => _selectedAddress;
            set => SetProperty(ref _selectedAddress, value);
        }
        public string Title { get; private set; }

        public bool IsEditMode => _clinicId.HasValue;

        // Komendy
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        // LoadDataCommand/InitializeAsync będzie wywoływane z OnAppearing strony

        public AddEditClinicViewModel(int? clinicId = null)
        {
            _clinicId = clinicId;
            _clinicService = DependencyService.Get<IClinicService>();
            _addressService = DependencyService.Get<IAddressService>();

            if (_clinicService == null || _addressService == null)
            {
                Console.WriteLine("KRYTYCZNY BŁĄD: Brak serwisu Clinic lub Address!");
                // Rozważ rzucenie wyjątku
            }

            Title = IsEditMode ? "Edytuj Klinikę" : "Dodaj Klinikę";
            AvailableAddresses = new ObservableCollection<AddressForView>();

            SaveCommand = new Command(async () => await ExecuteSaveCommand(), CanExecuteSaveCommand);
            DeleteCommand = new Command(async () => await ExecuteDeleteCommand(), CanExecuteDeleteCommand);

            // Nasłuchiwanie zmian do aktualizacji CanExecute
            PropertyChanged += (_, args) => {
                if (args.PropertyName == nameof(Name) ||
                    args.PropertyName == nameof(SelectedAddress) ||
                    args.PropertyName == nameof(IsBusy))
                {
                    (SaveCommand as Command)?.ChangeCanExecute();
                    (DeleteCommand as Command)?.ChangeCanExecute();
                }
            };
        }

        // Metoda inicjalizująca, wywoływana z OnAppearing strony
        public async Task InitializeAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // Zawsze ładuj dostępne adresy
                AvailableAddresses.Clear();
                var addresses = await _addressService.GetItemsAsync(true); // Wymuś odświeżenie adresów
                if (addresses != null)
                {
                    foreach (var addr in addresses)
                    {
                        AvailableAddresses.Add(addr);
                    }
                }

                // Jeśli jesteśmy w trybie edycji, załaduj dane kliniki
                if (IsEditMode && _clinicId.HasValue)
                {
                    var clinic = await _clinicService.GetItemAsync(_clinicId.Value);
                    if (clinic != null)
                    {
                        Name = clinic.Name;
                        // Znajdź i ustaw wybrany adres w Pickerze
                        SelectedAddress = AvailableAddresses.FirstOrDefault(a => a.AddressId == clinic.AddressId);
                        Title = $"Edytuj: {Name}";
                        OnPropertyChanged(nameof(Title)); // Zaktualizuj tytuł po załadowaniu nazwy
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Błąd", "Nie znaleziono kliniki do edycji.", "OK");
                        await PopPageAsync(); // Wróć, jeśli nie ma co edytować
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd inicjalizacji AddEditClinicViewModel: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można załadować danych formularza.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanExecuteSaveCommand()
        {
            return !IsBusy &&
                   !string.IsNullOrWhiteSpace(Name) &&
                   SelectedAddress != null; // Upewnij się, że adres jest wybrany
        }

        async Task ExecuteSaveCommand()
        {
            if (!CanExecuteSaveCommand())
            {
                await Application.Current.MainPage.DisplayAlert("Błąd Walidacji", "Nazwa kliniki oraz adres są wymagane.", "OK");
                return;
            }

            IsBusy = true;
            NotifyCommandCanExecuteChanged();

            try
            {
                // Używamy ClinicCreateDto dla obu operacji, zgodnie z definicją w API
                var clinicDto = new ClinicCreateDto
                {
                    Name = this.Name.Trim(),
                    AddressId = SelectedAddress.AddressId
                };

                string successMessage = "";
                string errorMessage = "";

                if (IsEditMode)
                {
                    await _clinicService.UpdateClinicAsync(_clinicId.Value, clinicDto);
                    successMessage = "Klinika została zaktualizowana.";
                    errorMessage = "Nie udało się zaktualizować kliniki."; // Ogólny, bo API powinno rzucić wyjątek
                }
                else
                {
                    var createdClinic = await _clinicService.CreateClinicAsync(clinicDto);
                    if (createdClinic == null) throw new Exception("API nie zwróciło utworzonej kliniki.");
                    successMessage = "Klinika została dodana.";
                    errorMessage = "Nie udało się dodać kliniki.";
                }

                await Application.Current.MainPage.DisplayAlert("Sukces", successMessage, "OK");
                await PopPageAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd zapisu kliniki: {ex.Message}");
                // Jeśli jest ApiException, można by wyświetlić apiEx.Content lub apiEx.StatusCode
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Wystąpił błąd podczas zapisu: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                NotifyCommandCanExecuteChanged();
            }
        }

        private bool CanExecuteDeleteCommand()
        {
            return !IsBusy && IsEditMode;
        }

        async Task ExecuteDeleteCommand()
        {
            if (!CanExecuteDeleteCommand()) return;

            bool confirmed = await Application.Current.MainPage.DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usunąć klinikę '{Name}'?", "Tak, usuń", "Anuluj");
            if (!confirmed) return;

            IsBusy = true;
            NotifyCommandCanExecuteChanged();

            try
            {
                await _clinicService.DeleteItemAsync(_clinicId.Value); // Używamy DeleteItemAsync z IDataStore
                await Application.Current.MainPage.DisplayAlert("Sukces", "Klinika została usunięta.", "OK");
                await PopPageAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd usuwania kliniki: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie udało się usunąć kliniki. Błąd: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                NotifyCommandCanExecuteChanged();
            }
        }

        private async Task PopPageAsync()
        {
            if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
            {
                await navigationPage.Navigation.PopAsync();
            }
        }

        private void NotifyCommandCanExecuteChanged()
        {
            (SaveCommand as Command)?.ChangeCanExecute();
            (DeleteCommand as Command)?.ChangeCanExecute();
        }

        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsBusy) ||
                propertyName == nameof(Name) ||
                propertyName == nameof(SelectedAddress))
            {
                NotifyCommandCanExecuteChanged();
            }
        }
    }
}