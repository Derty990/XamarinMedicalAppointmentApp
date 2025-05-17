using MedicalAppointmentApp.Models;
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
    public class DoctorsViewModel : BaseViewModel
    {
        private readonly IDoctorService _doctorService;
        public string Title { get; }
        public ObservableCollection<DoctorListItemDto> Doctors { get; }
        public ICommand LoadDoctorsCommand { get; }
        public ICommand EditDoctorCommand { get; }
        public ICommand AddDoctorCommand { get; }
        private DoctorListItemDto _selectedDoctor;
        public DoctorListItemDto SelectedDoctor
        {
            get => _selectedDoctor;
            set
            {
                // SetProperty powinno pochodzić z Twojego BaseViewModel i obsługiwać INotifyPropertyChanged
                // oraz zwracać true, jeśli wartość faktycznie się zmieniła.
                if (SetProperty(ref _selectedDoctor, value))
                {
                    // Aktualizuj stan CanExecute komendy za każdym razem, gdy zmienia się zaznaczenie.
                    // Komenda sama zdecyduje, czy może być wykonana na podstawie nowej wartości SelectedDoctor.
                    (EditDoctorCommand as Command)?.ChangeCanExecute();

                    // Jeśli nowo wybrany element nie jest null, automatycznie spróbuj wykonać komendę.
                    // Jeśli użytkownik odznaczył element (klikając ponownie lub CollectionView to robi),
                    // value będzie null i komenda nie zostanie wykonana.
                    if (value != null)
                    {
                        EditDoctorCommand.Execute(value);
                    }
                }
            }
        }
        public DoctorsViewModel()
        {
            Title = "Lekarze";
            _doctorService = DependencyService.Get<IDoctorService>();
            if (_doctorService == null)
            {
                Console.WriteLine("KRYTYCZNY BŁĄD: Nie zarejestrowano IDoctorService!");
            }

            Doctors = new ObservableCollection<DoctorListItemDto>();

            LoadDoctorsCommand = new Command(async () => await ExecuteLoadDoctorsCommand(), () => !IsBusy);
            // CanExecute dla EditDoctorCommand sprawdza, czy nie jest IsBusy ORAZ czy doctor (parametr) nie jest null.
            EditDoctorCommand = new Command<DoctorListItemDto>(async (doctor) => await ExecuteEditDoctorCommand(doctor), (doctor) => !IsBusy && doctor != null);
            AddDoctorCommand = new Command(async () => await ExecuteAddDoctorCommand(), () => !IsBusy);

            LoadDoctorsCommand.Execute(null);
        }

        // Metoda asynchroniczna do ładowania listy lekarzy
        async Task ExecuteLoadDoctorsCommand()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                Doctors.Clear();
                var items = await _doctorService.GetDoctorListItemsAsync();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Doctors.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas ładowania lekarzy: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można załadować listy lekarzy.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        async Task ExecuteAddDoctorCommand()
        {
            if (IsBusy) return;

            try
            {
                if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                {
                    await navigationPage.Navigation.PushAsync(new AddDoctorPage());
                }
                else
                {
                    Debug.WriteLine("Błąd nawigacji do dodawania: Struktura MainPage jest niepoprawna.");
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Wystąpił wewnętrzny błąd nawigacji (struktura strony).", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Nieudana nawigacja do AddDoctorPage: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można otworzyć strony dodawania lekarza.", "OK");
            }
        }

        // Metoda asynchroniczna do nawigacji do strony edycji lekarza
        async Task ExecuteEditDoctorCommand(DoctorListItemDto doctor)
        {
            // Warunek 'doctor == null' jest już sprawdzany przez CanExecute komendy.
            // 'IsBusy' również. Można by je tu pominąć, ale dla pewności można zostawić.
            if (doctor == null || IsBusy)
            {
                // Jeśli komenda została wywołana z nullem (np. przez odznaczenie),
                // a SelectedDoctor jest już nullem, nic nie rób.
                // Jeśli SelectedDoctor nie jest nullem, a dostajemy null, to znaczy, że odznaczamy.
                if (doctor == null && _selectedDoctor != null)
                {
                    SelectedDoctor = null; // Upewnij się, że SelectedDoctor jest null i CanExecute się odświeży.
                }
                return;
            }

            IsBusy = true; // Ustaw IsBusy, CanExecute komend się zaktualizuje przez OnPropertyChanged

            try
            {
                Debug.WriteLine($"Nawigacja do EditDoctorPage dla Doctor ID: {doctor.DoctorId}");
                if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                {
                    await navigationPage.Navigation.PushAsync(new EditDoctorPage(doctor.DoctorId));
                }
                else
                {
                    Debug.WriteLine("Błąd nawigacji do edycji: Struktura MainPage jest niepoprawna.");
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Wystąpił wewnętrzny błąd nawigacji.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Nieudana nawigacja do EditDoctorPage: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można otworzyć strony edycji lekarza.", "OK");
            }
            finally
            {
                IsBusy = false; // Zresetuj IsBusy
                                // WAŻNE: Zresetuj SelectedDoctor na null TUTAJ, po zakończeniu nawigacji.
                                // To pozwoli na ponowne wybranie tego samego elementu listy.
                SelectedDoctor = null;
            }
        }


        // Metoda wywoływana z code-behind strony, gdy ta się pojawia
        public void OnAppearing()
        {
            IsBusy = false;
            SelectedDoctor = null;
            LoadDoctorsCommand.Execute(null); // Odśwież listę przy powrocie
        }

        // Metoda z BaseViewModel, która aktualizuje CanExecute komend, gdy IsBusy się zmienia
        // Dodaliśmy też sprawdzanie dla EditDoctorCommand, gdy SelectedDoctor się zmienia
        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsBusy))
            {
                // Aktualizuj stan wszystkich komend zależnych od IsBusy
                (LoadDoctorsCommand as Command)?.ChangeCanExecute();
                (EditDoctorCommand as Command)?.ChangeCanExecute();
                (AddDoctorCommand as Command)?.ChangeCanExecute();
            }
            else if (propertyName == nameof(SelectedDoctor))
            {
                // Aktualizuj stan komendy edycji, gdy zmienia się zaznaczenie
                // (może być wyłączona, jeśli SelectedDoctor jest null)
                (EditDoctorCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}