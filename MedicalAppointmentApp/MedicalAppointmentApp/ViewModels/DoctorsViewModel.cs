using MedicalAppointmentApp.Models; // Dla DoctorListItemDto (jeśli tam jest)
using MedicalAppointmentApp.Services.Abstract; // Dla IDoctorService
using MedicalAppointmentApp.Views;             // Dla typów stron (AddDoctorPage, EditDoctorPage)
using MedicalAppointmentApp.XamarinApp.ApiClient; // Dla DoctorListItemDto (jeśli tam jest)
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

// Upewnij się, że ten namespace jest poprawny dla Twojego projektu
namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class DoctorsViewModel : BaseViewModel // Dziedziczy z Twojego BaseViewModel
    {
        // Prywatne pole dla serwisu lekarzy
        private readonly IDoctorService _doctorService;

        // Tytuł strony (może być ustawiony na stałe lub dynamicznie)
        public string Title { get; }

        // Kolekcja lekarzy do wyświetlenia w CollectionView/ListView
        // Używamy ObservableCollection, aby UI automatycznie reagowało na zmiany w kolekcji
        public ObservableCollection<DoctorListItemDto> Doctors { get; }

        // Komenda do ładowania/odświeżania listy lekarzy
        public ICommand LoadDoctorsCommand { get; }
        // Komenda do nawigacji do strony edycji po wybraniu lekarza
        public ICommand EditDoctorCommand { get; }
        // Komenda do nawigacji do strony dodawania nowego lekarza
        public ICommand AddDoctorCommand { get; }

        // Właściwość przechowująca aktualnie wybranego lekarza z listy
        // Używana przez CollectionView/ListView i komendę EditDoctorCommand
        private DoctorListItemDto _selectedDoctor;
        public DoctorListItemDto SelectedDoctor
        {
            get => _selectedDoctor;
            set
            {
                // Używamy SetProperty z BaseViewModel do ustawienia wartości i powiadomienia UI
                // Sprawdzamy też, czy nowa wartość jest inna niż stara
                if (SetProperty(ref _selectedDoctor, value) && value != null)
                {
                    // Jeśli ustawiono nowego lekarza (nie null), automatycznie wywołaj komendę edycji
                    EditDoctorCommand.Execute(value);
                }
            }
        }

        // Konstruktor ViewModelu
        public DoctorsViewModel()
        {
            Title = "Lekarze"; // Ustawienie tytułu strony

            // Pobranie instancji serwisu lekarzy za pomocą DependencyService
            // (Upewnij się, że IDoctorService i DoctorDataStore są zarejestrowane w App.xaml.cs)
            _doctorService = DependencyService.Get<IDoctorService>();
            if (_doctorService == null)
            {
                Console.WriteLine("KRYTYCZNY BŁĄD: Nie zarejestrowano IDoctorService!");
                // W produkcyjnej aplikacji należałoby obsłużyć ten błąd bardziej elegancko
            }

            // Inicjalizacja kolekcji lekarzy
            Doctors = new ObservableCollection<DoctorListItemDto>();

            // Inicjalizacja komend
            // new Command(async () => await MetodaWykonujaca(), () => WarunekCanExecute);
            LoadDoctorsCommand = new Command(async () => await ExecuteLoadDoctorsCommand(), () => !IsBusy);
            EditDoctorCommand = new Command<DoctorListItemDto>(async (doctor) => await ExecuteEditDoctorCommand(doctor), (doctor) => !IsBusy && doctor != null); // CanExecute sprawdza też, czy doctor nie jest null
            AddDoctorCommand = new Command(async () => await ExecuteAddDoctorCommand(), () => !IsBusy);

            // Wywołanie ładowania lekarzy od razu po utworzeniu ViewModelu
            // Execute(null) jest bezpieczne dla Command, jeśli nie oczekuje parametru
            LoadDoctorsCommand.Execute(null);
        }

        // Metoda asynchroniczna do ładowania listy lekarzy
        async Task ExecuteLoadDoctorsCommand()
        {
           
            if (IsBusy)
                return;

            
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
                // Sprawdź, czy struktura strony jest poprawna (Flyout + NavigationPage w Detail)
                if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                {
                    // Użyj stosu nawigacji strony Detail, aby otworzyć AddDoctorPage
                    await navigationPage.Navigation.PushAsync(new AddDoctorPage());
                }
                else
                {
                    // Błąd struktury - nie powinno się zdarzyć
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
            // Sprawdzenie, czy przekazano lekarza i czy nie trwa inna operacja
            if (doctor == null || IsBusy)
                return;

            try
            {
                Debug.WriteLine($"Nawigacja do EditDoctorPage dla Doctor ID: {doctor.DoctorId}");
                // Sprawdź poprawność struktury strony
                if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                {
                    // Przejdź do strony edycji, przekazując ID lekarza do konstruktora
                    await navigationPage.Navigation.PushAsync(new EditDoctorPage(doctor.DoctorId));
                }
                else
                {
                    Debug.WriteLine("Błąd nawigacji do edycji: Struktura MainPage jest niepoprawna.");
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Wystąpił wewnętrzny błąd nawigacji (struktura strony).", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Nieudana nawigacja do EditDoctorPage: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można otworzyć strony edycji lekarza.", "OK");
            }
            finally
            {
                // WAŻNE: Zresetuj SelectedDoctor z powrotem na null po próbie nawigacji.
                // To pozwoli na ponowne wybranie tego samego elementu, jeśli użytkownik wróci
                // i zapobiegnie pozostaniu elementu "zaznaczonego" wizualnie w CollectionView.
                SelectedDoctor = null;
            }
        }

        // Metoda wywoływana z code-behind strony, gdy ta się pojawia
        public void OnAppearing()
        {
            IsBusy = false; // Upewnij się, że flaga IsBusy jest zresetowana
            SelectedDoctor = null; // Wyczyść zaznaczenie lekarza
            // Możesz tutaj dodać logikę odświeżania danych, jeśli jest potrzebna za każdym razem
            // np. LoadDoctorsCommand.Execute(null);
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