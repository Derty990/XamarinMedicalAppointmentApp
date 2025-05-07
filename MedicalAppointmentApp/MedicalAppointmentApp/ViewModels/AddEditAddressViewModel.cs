using MedicalAppointmentApp.Services.Abstract; 
using MedicalAppointmentApp.XamarinApp.ApiClient; 
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class AddEditAddressViewModel : BaseViewModel
    {
        private readonly IAddressService _addressService;
        private readonly int? _addressId; 

        private string _street;
        public string Street
        {
            get => _street;
            set => SetProperty(ref _street, value);
        }

        private string _city;
        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        private string _postalCode;
        public string PostalCode
        {
            get => _postalCode;
            set => SetProperty(ref _postalCode, value);
        }
        public string Title { get; private set; }
        public bool IsEditMode => _addressId.HasValue;
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        private Command<int> LoadAddressCommand { get; }

        public AddEditAddressViewModel(int? addressId = null)
        {
            _addressId = addressId;
            _addressService = DependencyService.Get<IAddressService>();
            if (_addressService == null)
            {
                Console.WriteLine("BŁĄD KRYTYCZNY: Nie zarejestrowano IAddressService!");
            }

            Title = IsEditMode ? "Edytuj Adres" : "Dodaj Adres";

            SaveCommand = new Command(async () => await ExecuteSaveCommand(), CanExecuteSaveCommand);
            DeleteCommand = new Command(async () => await ExecuteDeleteCommand(), CanExecuteDeleteCommand);
            LoadAddressCommand = new Command<int>(async (id) => await ExecuteLoadAddressCommand(id));
            PropertyChanged += (_, args) => {
                if (args.PropertyName == nameof(Street) || args.PropertyName == nameof(City) || args.PropertyName == nameof(PostalCode) || args.PropertyName == nameof(IsBusy))
                {
                    (SaveCommand as Command)?.ChangeCanExecute();
                }
            };
        }
        public async Task InitializeAsync()
        {
            if (IsEditMode && _addressId.HasValue)
            {
                await ExecuteLoadAddressCommand(_addressId.Value);
            }
        }
        private async Task ExecuteLoadAddressCommand(int addressId)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var address = await _addressService.GetItemAsync(addressId);
                if (address != null)
                {
                    Street = address.Street;
                    City = address.City;
                    PostalCode = address.PostalCode;
                    Title = $"Edytuj: {Street}"; 
                    OnPropertyChanged(nameof(Title));
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Nie znaleziono adresu do edycji.", "OK");
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania adresu: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można załadować danych adresu.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Sprawdzenie, czy można zapisać
        private bool CanExecuteSaveCommand()
        {
            return !IsBusy &&
                   !string.IsNullOrWhiteSpace(Street) &&
                   !string.IsNullOrWhiteSpace(City) &&
                   !string.IsNullOrWhiteSpace(PostalCode);
        }

        // Zapis (Dodanie lub Aktualizacja)
        private async Task ExecuteSaveCommand()
        {
            if (!CanExecuteSaveCommand())
            {
                await Application.Current.MainPage.DisplayAlert("Błąd Walidacji", "Wszystkie pola są wymagane.", "OK");
                return;
            }

            IsBusy = true;
            (SaveCommand as Command)?.ChangeCanExecute();

            try 
            {
                
                var addressDto = new AddressCreateDto
                {
                    Street = this.Street.Trim(),
                    City = this.City.Trim(),
                    PostalCode = this.PostalCode.Trim()
                };

                bool success = false;
                string successMessage = "";
                string errorMessage = "";

                try 
                {
                    if (IsEditMode)
                    {
                        // UpdateAddressAsync zwraca Task, sukces = brak wyjątku
                        await _addressService.UpdateAddressAsync(_addressId.Value, addressDto);
                        success = true; // Załóż sukces, jeśli nie było wyjątku
                        successMessage = "Adres został zaktualizowany.";
                        errorMessage = "Nie udało się zaktualizować adresu.";
                    }
                    else
                    {
                        // CreateAddressAsync zwraca Task<AddressForView>
                        var createdAddress = await _addressService.CreateAddressAsync(addressDto);
                        success = createdAddress != null; // Sukces, jeśli API zwróciło obiekt
                        successMessage = "Adres został dodany.";
                        errorMessage = "Nie udało się dodać adresu.";
                    }
                }
                catch (Exception ex) // Złap błędy API lub inne
                {
                    Debug.WriteLine($"Błąd zapisu adresu: {ex.Message}");
                    errorMessage = $"Wystąpił błąd: {ex.Message}";
                    success = false;
                }
                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Sukces", successMessage, "OK");
                    // Poprawna nawigacja wstecz
                    if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                    {
                        await navigationPage.Navigation.PopAsync();
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", errorMessage, "OK");
                }
            }
            finally
            {
                IsBusy = false;
                (SaveCommand as Command)?.ChangeCanExecute();
            }
        }

        private bool CanExecuteDeleteCommand()
        {
            // Można usunąć tylko w trybie edycji i gdy nie trwa inna operacja
            return !IsBusy && IsEditMode;
        }

        async Task ExecuteDeleteCommand()
        {
            if (!CanExecuteDeleteCommand()) return;

            bool confirmed = await Application.Current.MainPage.DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usunąć adres: {Street}, {City}, {PostalCode}?", "Tak, usuń", "Anuluj");
            if (!confirmed) return;

            IsBusy = true;
            NotifyCommandCanExecuteChanged();

            try
            {
                // DeleteItemAsync zwraca Task, zakładamy sukces jeśli nie ma wyjątku
                await _addressService.DeleteItemAsync(_addressId.Value);

                await Application.Current.MainPage.DisplayAlert("Sukces", "Adres został usunięty.", "OK");
                await PopPageAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd usuwania adresu: {ex.Message}");
                // Można tu dodać bardziej szczegółową obsługę, np. jeśli API zwróci 409 Conflict
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie udało się usunąć adresu. ({ex.Message})", "OK");
            }
            finally
            {
                IsBusy = false;
                NotifyCommandCanExecuteChanged();
            }
        }

        private void NotifyCommandCanExecuteChanged()
        {
            (SaveCommand as Command)?.ChangeCanExecute();
            (DeleteCommand as Command)?.ChangeCanExecute();
        }

        private async Task PopPageAsync()
        {
            if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
            {
                await navigationPage.Navigation.PopAsync();
            }
        }

        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsBusy) ||
                propertyName == nameof(Street) ||
                propertyName == nameof(City) ||
                propertyName == nameof(PostalCode))
            {
                NotifyCommandCanExecuteChanged();
            }
        }
    }
}