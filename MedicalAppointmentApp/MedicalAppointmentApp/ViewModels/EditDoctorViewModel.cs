using MedicalAppointmentApp.Models;
using MedicalAppointmentApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient;
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class EditDoctorViewModel : BaseViewModel
    {
        private readonly IDoctorService _doctorService;
        private readonly ISpecializationService _specializationService;

        private int _doctorId;
        private string _firstName;
        public string FirstName { get => _firstName; private set => SetProperty(ref _firstName, value); }
        private string _lastName;
        public string LastName { get => _lastName; private set => SetProperty(ref _lastName, value); }
        private string _email;
        public string Email { get => _email; private set => SetProperty(ref _email, value); }
        public string Title { get; private set; }

        // Lista i wybrana specjalizacja do edycji
        public ObservableCollection<SpecializationSelectItem> AvailableSpecializations { get; }
        private int _selectedSpecializationIndex = -1;
        public int SelectedSpecializationIndex
        {
            get => _selectedSpecializationIndex;
            set => SetProperty(ref _selectedSpecializationIndex, value);
        }
        public ICommand LoadDataCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand DeleteDoctorCommand { get; }

        public EditDoctorViewModel()
        {
            Title = "Edytuj Lekarza";
            _doctorService = DependencyService.Get<IDoctorService>();
            _specializationService = DependencyService.Get<ISpecializationService>();

            if (_specializationService == null || _doctorService == null)
            {
                 Console.WriteLine("FATAL ERROR: Doctor or Specialization service not registered!");
            }

            AvailableSpecializations = new ObservableCollection<SpecializationSelectItem>();

            // Definicje komend
            LoadDataCommand = new Command<int>(async (id) => await ExecuteLoadDataCommand(id));
            SaveChangesCommand = new Command(async () => await ExecuteSaveChangesCommand(), () => !IsBusy);
            DeleteDoctorCommand = new Command(async () => await ExecuteDeleteDoctorCommand(), () => !IsBusy);
        }

        // Ładowanie danych lekarza i specjalizacji
        // Wywoływane z code-behind z przekazanym ID
        public async Task ExecuteLoadDataCommand(int doctorId)
        {
            if (IsBusy) return;
            _doctorId = doctorId; // Zapisz ID do późniejszego użytku
            IsBusy = true;
            try
            {
                AvailableSpecializations.Clear();
                SelectedSpecializationIndex = -1; // Resetuj wybór

                // Pobierz dane edytowanego lekarza (potrzebujemy pełnego DoctorForView)
                var doctor = await _doctorService.GetItemAsync(_doctorId);
                if (doctor != null)
                {
                    // Ustaw pola tylko do odczytu
                    FirstName = doctor.FirstName;
                    LastName = doctor.LastName;
                    Email = doctor.Email; // Zakładając, że DoctorForView ma te pola
                    Title = $"Edytuj: {FirstName} {LastName}"; 
                }
                else
                {
                     await Application.Current.MainPage.DisplayAlert("Błąd", "Nie znaleziono lekarza.", "OK");
                     await Application.Current.MainPage.Navigation.PopAsync(); // Wróć, jeśli nie ma lekarza
                     return;
                }

                // Pobierz listę specjalizacji
                var specializations = await _specializationService.GetItemsAsync(true);
                int initialSelectedIndex = -1;
                if (specializations != null)
                {
                    int currentIndex = 0;
                    foreach (var spec in specializations)
                    {
                         var specItem = new SpecializationSelectItem { SpecializationId = spec.SpecializationId, Name = spec.Name };
                         AvailableSpecializations.Add(specItem);
                         // Sprawdź, czy to jest obecna specjalizacja lekarza, aby ją zaznaczyć
                         if (spec.SpecializationId == doctor.SpecializationId)
                         {
                              initialSelectedIndex = currentIndex;
                         }
                         currentIndex++;
                    }
                }
                // Ustaw początkowy wybrany indeks dla Pickera
                SelectedSpecializationIndex = initialSelectedIndex;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading doctor/specializations for edit: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można załadować danych do edycji.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        // Zapis zmian (tylko specjalizacji)
        async Task ExecuteSaveChangesCommand()
        {
            if (SelectedSpecializationIndex < 0 || SelectedSpecializationIndex >= AvailableSpecializations.Count)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wybierz nową specjalizację.", "OK");
                return;
            }

            if (IsBusy) return;

            IsBusy = true;
            (SaveChangesCommand as Command)?.ChangeCanExecute();
            (DeleteDoctorCommand as Command)?.ChangeCanExecute();

            bool success = false; // Flaga sukcesu

            try
            {
                var selectedSpec = AvailableSpecializations[SelectedSpecializationIndex];
                var updateDto = new DoctorUpdateDto
                {
                    SpecializationId = selectedSpec.SpecializationId
                };

                try 
                {
                    await _doctorService.UpdateDoctorSpecializationAsync(_doctorId, updateDto);
                    success = true; 
                }
                catch (ApiException apiEx) when (apiEx.StatusCode == 204)
                {
                    Console.WriteLine($"API returned 204 No Content for Doctor PUT. Treating as success.");
                    success = true;
                }
                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Sukces", "Specjalizacja lekarza zaktualizowana.", "OK");
                    if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                    {
                        await navigationPage.Navigation.PopAsync();
                    }
                    else
                    {
                        Debug.WriteLine("Error navigating back after save: MainPage structure incorrect.");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Nie udało się zaktualizować specjalizacji.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
                (SaveChangesCommand as Command)?.ChangeCanExecute();
                (DeleteDoctorCommand as Command)?.ChangeCanExecute();
            }
        }

        // Usuwanie lekarza
        async Task ExecuteDeleteDoctorCommand()
        {
            if (IsBusy) return;

            
            bool confirmed = await Application.Current.MainPage.DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usunąć lekarza {FirstName} {LastName}?", "Tak, usuń", "Anuluj");
            if (!confirmed) return;

            IsBusy = true;
            (SaveChangesCommand as Command)?.ChangeCanExecute();
            (DeleteDoctorCommand as Command)?.ChangeCanExecute();

            try 
            {
                await _doctorService.DeleteItemAsync(_doctorId);

                await Application.Current.MainPage.DisplayAlert("Sukces", "Lekarz został usunięty.", "OK");

                if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                {
                    await navigationPage.Navigation.PopAsync(); 
                }
                else
                {
                    Debug.WriteLine("Error navigating back after delete: MainPage structure incorrect.");
                }
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Error deleting doctor: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie udało się usunąć lekarza. Sprawdź połączenie lub czy lekarz nie ma powiązanych wizyt.", "OK");
            }
            finally 
            {
                IsBusy = false;
                (SaveChangesCommand as Command)?.ChangeCanExecute();
                (DeleteDoctorCommand as Command)?.ChangeCanExecute();
            }
        }

        // OnPropertyChanged dla aktualizacji CanExecute
        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
         {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsBusy))
            {
                (SaveChangesCommand as Command)?.ChangeCanExecute();
                (DeleteDoctorCommand as Command)?.ChangeCanExecute();
            }
         }
    }
}