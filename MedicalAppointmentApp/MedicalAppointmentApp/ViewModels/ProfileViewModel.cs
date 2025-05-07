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
    public class ProfileViewModel : BaseViewModel
    {
        public string Title { get; }

        private readonly IUserService _userService;
        private readonly IAddressService _addressService; 

        private UserForView _userProfile;
        public UserForView UserProfile
        {
            get => _userProfile;
            private set => SetProperty(ref _userProfile, value);
        }

        private AddressForView _userAddress;
        public AddressForView UserAddress
        {
            get => _userAddress;
            private set => SetProperty(ref _userAddress, value);
        }

        public ICommand LoadProfileCommand { get; }
        public ICommand EditProfileCommand { get; }

        public ProfileViewModel()
        {
            Title = "Mój Profil";
            _userService = DependencyService.Get<IUserService>();
            _addressService = DependencyService.Get<IAddressService>(); 

            if (_userService == null || _addressService == null)
            {
                Console.WriteLine("BŁĄD: IUserService lub IAddressService nie jest zarejestrowany!");
            }

            LoadProfileCommand = new Command(async () => await ExecuteLoadProfileCommand());
            EditProfileCommand = new Command(async () => await ExecuteEditProfileCommand(), () => !IsBusy && UserProfile != null);
        }

        async Task ExecuteLoadProfileCommand()
        {
            if (IsBusy) return;
            IsBusy = true;
            (EditProfileCommand as Command)?.ChangeCanExecute();

            try
            {
                UserProfile = null; 
                UserAddress = null; 

                if (Application.Current.Properties.TryGetValue("LoggedInUserId", out object userIdObj) && userIdObj is int userId)
                {
                    var loadedUser = await _userService.GetItemAsync(userId);
                    if (loadedUser != null)
                    {
                        UserProfile = loadedUser; 
                        // Jeśli użytkownik ma przypisany AddressId, pobierz dane adresu
                        if (UserProfile.AddressId.HasValue)
                        {
                            UserAddress = await _addressService.GetItemAsync(UserProfile.AddressId.Value);
                            // Jeśli adres nie został znaleziony, UserAddress pozostanie null, co jest OK
                            // XAML obsłuży wyświetlanie braku danych
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można załadować danych użytkownika.", "OK");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można zidentyfikować zalogowanego użytkownika.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania profilu: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można załadować danych profilu.", "OK");
            }
            finally
            {
                IsBusy = false;
                (EditProfileCommand as Command)?.ChangeCanExecute(); 
            }
        }

        async Task ExecuteEditProfileCommand()
        {
            if (UserProfile == null || IsBusy) return;

            if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
            {
                await navigationPage.Navigation.PushAsync(new EditProfilePage(UserProfile.UserId));
            }
        }

        public void OnAppearing()
        {
            IsBusy = false; 
            LoadProfileCommand.Execute(null);
        }

        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsBusy) || propertyName == nameof(UserProfile))
            {
                (EditProfileCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}