using MedicalAppointmentApp.Services.Abstract; 
using MedicalAppointmentApp.Views;            
using MedicalAppointmentApp.XamarinApp.ApiClient; 
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class LoginViewModel : BaseViewModel 
    {
        private readonly IUserService _userService;

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        public LoginViewModel()
        {
            // Pobierz serwis 
            _userService = DependencyService.Get<IUserService>();
            if (_userService == null)
            {
                // Prosty komunikat lub rzucenie wyjątku, jeśli konfiguracja jest błędna
                Console.WriteLine("KRYTYCZNY BŁĄD: Nie zarejestrowano IUserService!");
                // throw new InvalidOperationException("IUserService not registered.");
            }

            // Inicjalizacja komend - powiązanie z IsBusy jest nadal przydatne
            LoginCommand = new Command(async () => await OnLoginClicked(), () => !IsBusy);
            RegisterCommand = new Command(async () => await OnRegisterClicked(), () => !IsBusy);
            ForgotPasswordCommand = new Command(async () => await OnForgotPasswordTapped(), () => !IsBusy);
         
        }

        // Metoda wywoływana przez LoginCommand - UPROSZCZONA
        private async Task OnLoginClicked()
        {
            if (IsBusy) return;

            // Podstawowa walidacja - zostawiamy
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź email i hasło.", "OK");
                return;
            }

            IsBusy = true;
            // Bezpośrednie odświeżenie stanu komend
            (LoginCommand as Command)?.ChangeCanExecute();
            (RegisterCommand as Command)?.ChangeCanExecute();
            (ForgotPasswordCommand as Command)?.ChangeCanExecute();

            UserForView loggedInUser = await _userService.LoginUserAsync(Email, Password);

            if (loggedInUser != null)
            {

                Application.Current.Properties["IsUserLoggedIn"] = true;
                Application.Current.Properties["LoggedInUserId"] = loggedInUser.UserId;
                Application.Current.Properties["LoggedInUserEmail"] = loggedInUser.Email;

                await Application.Current.SavePropertiesAsync();

                
                Email = string.Empty;
                Password = string.Empty;

                bool needsInvoke = Xamarin.Forms.Device.IsInvokeRequired; // Powinno być FALSE
                System.Diagnostics.Debug.WriteLine($"[LoginViewModel] Czy potrzebny Invoke przed ustawieniem MainPage? {needsInvoke}");
                if (needsInvoke)
                {
                    // To nie powinno się zdarzyć przy komendzie, ale sprawdzamy
                    await Application.Current.MainPage.DisplayAlert("Błąd Wątku!", "Próba ustawienia MainPage z wątku tła!", "OK");
                    IsBusy = false;
                    // Odśwież CanExecute
                    (LoginCommand as Command)?.ChangeCanExecute();
                    (RegisterCommand as Command)?.ChangeCanExecute();
                    (ForgotPasswordCommand as Command)?.ChangeCanExecute();
                    return; // Nie kontynuuj, jeśli nie jesteśmy w głównym wątku
                }

                Application.Current.MainPage = new MainFlyoutPage();

                /*              TO BYŁO NA POTRZEBY TESTÓW 
                 * 
                 * Application.Current.MainPage = new ContentPage
                {
                    BackgroundColor = Color.LightGreen,
                    Content = new StackLayout
                    {
                        VerticalOptions = LayoutOptions.Center,
                        Children = {
                new Label { Text = "Logowanie OK!", FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), HorizontalOptions = LayoutOptions.Center },
                new Label { Text = $"UserID: {loggedInUser.UserId}", HorizontalOptions = LayoutOptions.Center },
                new Label { Text = $"Email: {loggedInUser.Email}", HorizontalOptions = LayoutOptions.Center }
            }
                    }
                };*/
            }
            else
            {
                // Logowanie nieudane (niepoprawne dane lub inny błąd zwrócony jako null przez serwis)
                await Application.Current.MainPage.DisplayAlert("Błąd logowania", "Niepoprawny email lub hasło.", "OK");
                Password = string.Empty; // Wyczyść tylko hasło
            }

            
            IsBusy = false;
            (LoginCommand as Command)?.ChangeCanExecute();
            (RegisterCommand as Command)?.ChangeCanExecute();
            (ForgotPasswordCommand as Command)?.ChangeCanExecute();
        }

        // Metoda wywoływana przez RegisterCommand
        private async Task OnRegisterClicked()
        {
            if (IsBusy) return;

            try
            {
               
                await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
            }
            catch (Exception ex)
            {
                // Prosta obsługa błędu, jeśli nawigacja się nie powiedzie
                System.Diagnostics.Debug.WriteLine($"Navigation to RegisterPage failed: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można otworzyć strony rejestracji.", "OK");
            }
        }

        // Metoda wywoływana przez ForgotPasswordCommand 
        private async Task OnForgotPasswordTapped()
        {
            if (IsBusy) return;
            await Application.Current.MainPage.DisplayAlert("Resetowanie hasła", "Funkcja resetowania hasła będzie dostępna wkrótce.", "OK");
        }

        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsBusy))
            {
                // Bezpośrednie odświeżenie stanu komend
                (LoginCommand as Command)?.ChangeCanExecute();
                (RegisterCommand as Command)?.ChangeCanExecute();
                (ForgotPasswordCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}