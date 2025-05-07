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
    public class EditProfileViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        private readonly int _userId; 

        private string _firstName;
        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }

        private string _lastName;
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }

        private string _email;
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Title { get; }
        public ObservableCollection<AddressForView> AvailableAddresses { get; }

        private AddressForView _selectedAddress;
        public AddressForView SelectedAddress
        {
            get => _selectedAddress;
            set => SetProperty(ref _selectedAddress, value);
        }

        public ICommand SaveChangesCommand { get; }

        public EditProfileViewModel(int userId)
        {
            _userId = userId;
            _userService = DependencyService.Get<IUserService>();
            _addressService = DependencyService.Get<IAddressService>();

            if (_userService == null || _addressService == null)
            {
                Console.WriteLine("BŁĄD KRYTYCZNY: Nie zarejestrowano IUserService lub IAddressService!");
            }

            Title = "Edytuj Profil";
            AvailableAddresses = new ObservableCollection<AddressForView>();

            SaveChangesCommand = new Command(async () => await ExecuteSaveChangesCommand(), CanExecuteSaveChangesCommand);

            PropertyChanged += (_, args) => {
                if (args.PropertyName == nameof(FirstName) ||
                    args.PropertyName == nameof(LastName) ||
                    args.PropertyName == nameof(Email) ||
                    args.PropertyName == nameof(IsBusy)) 
                {
                    (SaveChangesCommand as Command)?.ChangeCanExecute();
                }
            };
        }
        public async Task InitializeAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            (SaveChangesCommand as Command)?.ChangeCanExecute(); 

            try
            {
                var user = await _userService.GetItemAsync(_userId);
                if (user != null)
                {
                    FirstName = user.FirstName;
                    LastName = user.LastName;
                    Email = user.Email;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można załadować danych profilu.", "OK");
                    await PopPageAsync();
                    return;
                }

                AvailableAddresses.Clear();
                var addresses = await _addressService.GetItemsAsync(true); // Wymuś odświeżenie
                if (addresses != null)
                {
                    foreach (var addr in addresses)
                    {
                        AvailableAddresses.Add(addr);
                    }
                    // Ustaw aktualnie wybrany adres, jeśli użytkownik go ma
                    if (user.AddressId.HasValue)
                    {
                        SelectedAddress = AvailableAddresses.FirstOrDefault(a => a.AddressId == user.AddressId.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania danych do edycji profilu: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można załadować formularza edycji.", "OK");
            }
            finally
            {
                IsBusy = false;
                (SaveChangesCommand as Command)?.ChangeCanExecute(); // Zaktualizuj stan przycisku
            }
        }

        private bool CanExecuteSaveChangesCommand()
        {
            return !IsBusy &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Email); // SelectedAddress może być null (użytkownik może nie mieć adresu)
        }

        private async Task ExecuteSaveChangesCommand()
        {
            if (!CanExecuteSaveChangesCommand())
            {
                await Application.Current.MainPage.DisplayAlert("Błąd Walidacji", "Imię, nazwisko i email są wymagane.", "OK");
                return;
            }

            IsBusy = true;
            (SaveChangesCommand as Command)?.ChangeCanExecute();

            try
            { 
                var userUpdateDto = new UserUpdateDto
                {
                    FirstName = this.FirstName.Trim(),
                    LastName = this.LastName.Trim(),
                    Email = this.Email.Trim(),
                    AddressId = this.SelectedAddress?.AddressId 
                };

                await _userService.UpdateUserAsync(_userId, userUpdateDto);
                if (Application.Current.Properties.ContainsKey("LoggedInUserId") &&
                    (int)Application.Current.Properties["LoggedInUserId"] == _userId)
                {
                    Application.Current.Properties["LoggedInUserFirstName"] = userUpdateDto.FirstName;
                    Application.Current.Properties["LoggedInUserLastName"] = userUpdateDto.LastName;
                    Application.Current.Properties["LoggedInUserEmail"] = userUpdateDto.Email;
                    // AddressId jest bardziej skomplikowane do zaktualizowania w Properties,
                    // bo potrzebowalibyśmy pełnego obiektu adresu lub osobnego zapytania.
                    // Na razie pominiemy aktualizację adresu w Properties.
                    await Application.Current.SavePropertiesAsync();
                }

                await Application.Current.MainPage.DisplayAlert("Sukces", "Profil został zaktualizowany.", "OK");
                await PopPageAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd aktualizacji profilu: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie udało się zaktualizować profilu. ({ex.Message})", "OK");
            }
            finally
            {
                IsBusy = false;
                (SaveChangesCommand as Command)?.ChangeCanExecute();
            }
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
                propertyName == nameof(FirstName) ||
                propertyName == nameof(LastName) ||
                propertyName == nameof(Email))
            {
                (SaveChangesCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}