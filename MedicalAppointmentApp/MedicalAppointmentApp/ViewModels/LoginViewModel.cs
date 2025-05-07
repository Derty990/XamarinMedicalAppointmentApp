using MedicalAppointmentApp.Services.Abstract;    
using MedicalAppointmentApp.Views;                
using MedicalAppointmentApp.XamarinApp.ApiClient; 
using System;
using System.Diagnostics; 
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService; // Serwis do obsługi logiki użytkownika (API)

        // Właściwości powiązane z polami Entry w LoginPage.xaml
        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value); // Metoda z BaseViewModel do ustawiania wartości i powiadamiania UI
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        // Komendy powiązane z przyciskami w LoginPage.xaml
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; } // Choć na razie tylko wyświetla alert

        // Konstruktor ViewModelu
        public LoginViewModel()
        {
            // Pobranie instancji serwisu użytkownika przez DependencyService
            // (IUserService musi być zarejestrowany w App.xaml.cs)
            _userService = DependencyService.Get<IUserService>();
            if (_userService == null)
            {
                // Krytyczny błąd konfiguracji - serwis nie został zarejestrowany
                Console.WriteLine("KRYTYCZNY BŁĄD: Nie zarejestrowano IUserService!");
                // W produkcyjnej aplikacji można by rzucić wyjątek lub wyświetlić globalny błąd
            }

            // Inicjalizacja komend
            // Każda komenda ma metodę wykonującą (async () => await ...)
            // oraz warunek CanExecute, który zależy od flagi !IsBusy (komenda nieaktywna, gdy IsBusy=true)
            LoginCommand = new Command(async () => await OnLoginClicked(), () => !IsBusy);
            RegisterCommand = new Command(async () => await OnRegisterClicked(), () => !IsBusy);
            ForgotPasswordCommand = new Command(async () => await OnForgotPasswordTapped(), () => !IsBusy);
        }

        // Metoda wywoływana po kliknięciu przycisku "Zaloguj się"
        private async Task OnLoginClicked()
        {
            // Jeśli operacja już trwa, nie rób nic
            if (IsBusy) return;

            // Podstawowa walidacja pól wejściowych
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź email i hasło.", "OK");
                return;
            }

            // Rozpocznij operację logowania
            IsBusy = true;
            NotifyCommandCanExecuteChanged(); // Poinformuj UI o zmianie stanu komend (staną się nieaktywne)

            UserForView loggedInUser = null; // Zmienna na wynik logowania

            try // Dodajemy podstawowy try-catch na wypadek błędów komunikacji z API
            {
                // Wywołaj metodę serwisu do logowania
                loggedInUser = await _userService.LoginUserAsync(Email, Password);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas logowania (serwis): {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd Logowania", "Wystąpił problem podczas próby logowania. Sprawdź połączenie.", "OK");
                // Ustaw loggedInUser na null, aby logika poniżej zadziałała poprawnie
                loggedInUser = null;
            }


            if (loggedInUser != null) // Jeśli logowanie się powiodło (serwis zwrócił dane użytkownika)
            {
                // Zapisz informacje o zalogowanym użytkowniku w globalnych właściwościach aplikacji
                Application.Current.Properties["IsUserLoggedIn"] = true;
                Application.Current.Properties["LoggedInUserId"] = loggedInUser.UserId;
                Application.Current.Properties["LoggedInUserEmail"] = loggedInUser.Email;
                Application.Current.Properties["LoggedInUserFirstName"] = loggedInUser.FirstName; // Dodaj jeśli UserForView to ma
                Application.Current.Properties["LoggedInUserLastName"] = loggedInUser.LastName;   // Dodaj jeśli UserForView to ma
                Application.Current.Properties["LoggedInUserRoleId"] = loggedInUser.RoleId;     // Dodaj jeśli UserForView to ma
                await Application.Current.SavePropertiesAsync(); // Zapisz właściwości na stałe

                // Wyczyść pola formularza logowania
                Email = string.Empty;
                Password = string.Empty;

                // Ustaw główną stronę aplikacji na MainFlyoutPage (główny interfejs po zalogowaniu)
                Application.Current.MainPage = new MainFlyoutPage();
            }
            else // Jeśli logowanie się nie powiodło (np. błędne dane, serwis zwrócił null)
            {
                // Jeśli nie było wyjątku, ale loggedInUser jest null, to znaczy, że dane były błędne
                if (!IsBusy) // Sprawdź czy IsBusy nie zostało zresetowane przez błąd w catch
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd logowania", "Niepoprawny email lub hasło.", "OK");
                    Password = string.Empty; // Wyczyść tylko hasło
                }
            }

            // Zakończ operację logowania
            IsBusy = false;
            NotifyCommandCanExecuteChanged(); // Poinformuj UI o zmianie stanu komend (staną się aktywne)
        }

        // Metoda wywoływana po kliknięciu przycisku "Zarejestruj się"
        private async Task OnRegisterClicked()
        {
            if (IsBusy) return; // Jeśli inna operacja trwa, nie rób nic

            try
            {
                // Użyj nawigacji stosowej, ponieważ LoginPage jest w NavigationPage
                await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Nawigacja do RegisterPage nie powiodła się: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można otworzyć strony rejestracji.", "OK");
            }
        }

        // Metoda wywoływana po kliknięciu "Nie pamiętasz hasła?"
        private async Task OnForgotPasswordTapped()
        {
            if (IsBusy) return; // Jeśli inna operacja trwa, nie rób nic
            // Placeholder - wyświetl komunikat
            await Application.Current.MainPage.DisplayAlert("Resetowanie hasła", "Funkcja resetowania hasła będzie dostępna wkrótce.", "OK");
        }

        // Metoda pomocnicza do odświeżania stanu CanExecute wszystkich komend
        private void NotifyCommandCanExecuteChanged()
        {
            (LoginCommand as Command)?.ChangeCanExecute();
            (RegisterCommand as Command)?.ChangeCanExecute();
            (ForgotPasswordCommand as Command)?.ChangeCanExecute();
        }

        // Przesłonięcie OnPropertyChanged z BaseViewModel do automatycznej aktualizacji CanExecute
        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsBusy)) // Jeśli zmieniła się właściwość IsBusy
            {
                NotifyCommandCanExecuteChanged(); // Zaktualizuj stan komend
            }
        }
    }
}