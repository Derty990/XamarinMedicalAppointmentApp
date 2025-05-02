using MedicalAppointmentApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient; 
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms; 

namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel 
    {
       
        private readonly IUserService _userService;

        

        private string _firstName;
        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }

        private string _lastName;
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }

        private string _email;
        public string Email { get => _email; set => SetProperty(ref _email, value); }

        private string _password;
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        private string _confirmPassword;
        public string ConfirmPassword { get => _confirmPassword; set => SetProperty(ref _confirmPassword, value); }

        private bool _termsAccepted;
        public bool TermsAccepted { get => _termsAccepted; set => SetProperty(ref _termsAccepted, value); }

        private bool _privacyAccepted;
        public bool PrivacyAccepted { get => _privacyAccepted; set => SetProperty(ref _privacyAccepted, value); }

      
        public ICommand RegisterCommand { get; }

        public RegisterViewModel(/* IUserService userService */)
        {
            // Pobierz serwis przez DependencyService
            _userService = DependencyService.Get<IUserService>();

            // Sprawdź JAWNIE, czy serwis został poprawnie pobrany
            if (_userService == null)
            {
                // Rzuć wyjątkiem, bo to oznacza błąd konfiguracji DI
                throw new InvalidOperationException($"Could not resolve {nameof(IUserService)}. Was it registered in App.xaml.cs?");
            }

            RegisterCommand = new Command(async () => await ExecuteRegisterCommand(), CanExecuteRegisterCommand);
            PropertyChanged += OnViewModelPropertyChanged;
        }

        // Wywoływane przy zmianie właściwości, aby odświeżyć stan przycisku
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Odśwież CanExecute dla RegisterCommand
            ((Command)RegisterCommand).ChangeCanExecute();
        }

        // Sprawdza, czy można wykonać komendę (np. czy przycisk ma być aktywny)
        private bool CanExecuteRegisterCommand()
        {
            return !IsBusy
                   && !string.IsNullOrWhiteSpace(FirstName)
                   && !string.IsNullOrWhiteSpace(LastName)
                   && !string.IsNullOrWhiteSpace(Email) 
                   && !string.IsNullOrWhiteSpace(Password)
                   && Password == ConfirmPassword 
                   && TermsAccepted
                   && PrivacyAccepted;
        }

        // Logika wykonywana po kliknięciu przycisku
        private async Task ExecuteRegisterCommand()
        {
            // Dodatkowe sprawdzenie, gdyby CanExecute się nie odświeżyło
            if (!CanExecuteRegisterCommand())
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wypełnij poprawnie wszystkie pola i zaakceptuj zgody.", "OK");
                return;
            }

            IsBusy = true;
            OnPropertyChanged(nameof(IsNotBusy)); 

            try
            {
                // Stwórz obiekt DTO wejściowego na podstawie danych z ViewModelu
                // Używamy klasy WYGENEROWANEJ przez narzędzie OpenAPI!
                var newUserDto = new UserCreateDto 
                {
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    Email = this.Email,
                    Password = this.Password, // API zajmie się hashowaniem
                    RoleId = 1, // Domyślnie rejestrujemy Pacjenta
                    AddressId = null // Na razie nie dodajemy adresu przy rejestracji
                };

                // Wywołaj metodę serwisu (DataStore), która wywoła API
                // Metoda RegisterUserAsync z UserDataStore powinna zwrócić wygenerowane UserForView lub null
                UserForView createdUser = await _userService.RegisterUserAsync(newUserDto);

                // Obsługa wyniku
                if (createdUser != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Sukces", $"Konto dla {createdUser.FirstName} zostało utworzone!", "OK");
                    // Nawigacja - np. powrót do strony logowania
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    // Błąd zgłoszony przez DataStore (który złapał błąd z API)
                    await Application.Current.MainPage.DisplayAlert("Błąd Rejestracji", "Nie udało się utworzyć konta. Sprawdź, czy podany email nie jest już zajęty lub spróbuj ponownie.", "OK");
                }
            }
            catch (Exception ex) // Złap nieoczekiwane błędy (np. sieciowe, jeśli DataStore ich nie złapał)
            {
                Debug.WriteLine($"[Register VM] Exception: {ex}");
                await Application.Current.MainPage.DisplayAlert("Błąd Krytyczny", "Wystąpił nieoczekiwany błąd podczas rejestracji.", "OK");
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsNotBusy)); // Powiadom UI
            }
        }
    }
}