using MedicalAppointmentApp.Services.Abstract; 
using MedicalAppointmentApp.Views;             
using MedicalAppointmentApp.XamarinApp.ApiClient; 
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class AddressesViewModel : BaseViewModel
    {
        public string Title { get; }

        private readonly IAddressService _addressService;

        public ObservableCollection<AddressForView> Addresses { get; }

        public ICommand LoadAddressesCommand { get; }
        public ICommand AddAddressCommand { get; }
        public ICommand EditAddressCommand { get; }
        public ICommand DeleteAddressCommand { get; }

        private AddressForView _selectedAddress;
        public AddressForView SelectedAddress
        {
            get => _selectedAddress;
            set
            {
                if (SetProperty(ref _selectedAddress, value) && value != null)
                {
                    // Wywołaj edycję po wybraniu elementu
                    EditAddressCommand.Execute(value);
                }
            }
        }

        public AddressesViewModel()
        {
            Title = "Adresy";
            _addressService = DependencyService.Get<IAddressService>();
            if (_addressService == null) { Console.WriteLine("BŁĄD: Nie zarejestrowano IAddressService!"); }

            Addresses = new ObservableCollection<AddressForView>();

            LoadAddressesCommand = new Command(async () => await ExecuteLoadAddressesCommand(), () => !IsBusy);
            AddAddressCommand = new Command(async () => await ExecuteAddAddressCommand(), () => !IsBusy);
            EditAddressCommand = new Command<AddressForView>(async (addr) => await ExecuteEditAddressCommand(addr), (addr) => !IsBusy && addr != null);
            
        }

        async Task ExecuteLoadAddressesCommand()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                Addresses.Clear();
                var items = await _addressService.GetItemsAsync(true);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Addresses.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania adresów: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można załadować listy adresów.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteAddAddressCommand()
        {
            if (IsBusy) return;
            await NavigateToEditPage(null); // Przekaż null jako ID dla trybu dodawania
        }

        async Task ExecuteEditAddressCommand(AddressForView address)
        {
            if (address == null || IsBusy) return;
            await NavigateToEditPage(address.AddressId); // Przekaż ID do edycji
        }

        // Wspólna metoda nawigacji do strony Add/Edit
        async Task NavigateToEditPage(int? addressId)
        {
            try
            {
                if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                {
                    
                    await navigationPage.Navigation.PushAsync(new AddEditAddressPage(addressId));
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Nawigacja do AddEditAddressPage nie powiodła się: {ex.Message}"); }
            finally
            {
                SelectedAddress = null; // Zawsze czyść zaznaczenie
            }
        }

        public void OnAppearing()
        {
            IsBusy = false;
            SelectedAddress = null;
            // Ładuj dane przy każdym pojawieniu się strony
            LoadAddressesCommand.Execute(null);
        }

        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsBusy))
            {
                (LoadAddressesCommand as Command)?.ChangeCanExecute();
                (AddAddressCommand as Command)?.ChangeCanExecute();
                (EditAddressCommand as Command)?.ChangeCanExecute();
            }
            else if (propertyName == nameof(SelectedAddress))
            {
                (EditAddressCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}