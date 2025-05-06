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
    public class AddDoctorViewModel : BaseViewModel
    {
        // Title - ustawiony w konstruktorze
        public string Title { get; }

        // Serwisy
        private readonly IDoctorService _doctorService;
        private readonly IUserService _userService;
        private readonly ISpecializationService _specializationService;

        // Kolekcje dla Pickerów (dane źródłowe i stringi do wyświetlania)
        public ObservableCollection<UserSelectItem> AvailableUsers { get; }
        public ObservableCollection<SpecializationSelectItem> AvailableSpecializations { get; }
        public ObservableCollection<string> UserDisplayNames { get; }
        public ObservableCollection<string> SpecializationNames { get; }

     
        private int _selectedUserIndex = -1;
        public int SelectedUserIndex
        {
            get => _selectedUserIndex;
            set
            {
                if (SetProperty(ref _selectedUserIndex, value))
                {
                   
                    SelectedUser = (_selectedUserIndex >= 0 && _selectedUserIndex < AvailableUsers.Count)
                                    ? AvailableUsers[_selectedUserIndex]
                                    : null;
                }
            }
        }

        private int _selectedSpecializationIndex = -1;
        public int SelectedSpecializationIndex
        {
            get => _selectedSpecializationIndex;
            set
            {
                if (SetProperty(ref _selectedSpecializationIndex, value))
                {
                   
                    SelectedSpecialization = (_selectedSpecializationIndex >= 0 && _selectedSpecializationIndex < AvailableSpecializations.Count)
                                            ? AvailableSpecializations[_selectedSpecializationIndex]
                                            : null;
                }
            }
        }

        public UserSelectItem SelectedUser { get; private set; }
        public SpecializationSelectItem SelectedSpecialization { get; private set; }

        public ICommand LoadDataCommand { get; }
        public ICommand SaveDoctorCommand { get; }

        // Konstruktor
        public AddDoctorViewModel()
        {
            Title = "Dodaj Lekarza";
            _doctorService = DependencyService.Get<IDoctorService>();
            _userService = DependencyService.Get<IUserService>();
            _specializationService = DependencyService.Get<ISpecializationService>();

            // Proste sprawdzenie, czy serwisy są dostępne
            if (_userService == null || _specializationService == null || _doctorService == null)
            {
                Console.WriteLine("KRYTYCZNY BŁĄD: Brak jednego lub więcej serwisów!");

            }

            AvailableUsers = new ObservableCollection<UserSelectItem>();
            AvailableSpecializations = new ObservableCollection<SpecializationSelectItem>();
            UserDisplayNames = new ObservableCollection<string>();
            SpecializationNames = new ObservableCollection<string>();

            LoadDataCommand = new Command(async () => await ExecuteLoadDataCommand());
            SaveDoctorCommand = new Command(async () => await ExecuteSaveDoctorCommand(), () => !IsBusy);

            LoadDataCommand.Execute(null);
        }

        async Task ExecuteLoadDataCommand()
        {
            if (IsBusy) return;
            IsBusy = true;
          
            try
            {
                AvailableUsers.Clear();
                AvailableSpecializations.Clear();
                UserDisplayNames.Clear();
                SpecializationNames.Clear();

                SelectedUserIndex = -1;
                SelectedSpecializationIndex = -1;

                var users = await _userService.GetItemsAsync(true);
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        var userItem = new UserSelectItem { UserId = user.UserId, FullName = $"{user.FirstName} {user.LastName}".Trim() };
                        AvailableUsers.Add(userItem);
                        UserDisplayNames.Add(userItem.FullName);
                    }
                }

                var specializations = await _specializationService.GetItemsAsync(true); 
                if (specializations != null)
                {
                    foreach (var spec in specializations)
                    {
                        var specItem = new SpecializationSelectItem { SpecializationId = spec.SpecializationId, Name = spec.Name };
                        AvailableSpecializations.Add(specItem);
                        SpecializationNames.Add(specItem.Name);
                    }
                }
            }
            finally
            {
                await Task.Delay(50);
                IsBusy = false;
            }
        }
        async Task ExecuteSaveDoctorCommand()
        {
            // 1. Walidacja danych wejściowych
            if (SelectedUser == null || SelectedSpecialization == null)
            {
                // Używamy Application.Current.MainPage do wyświetlania alertów
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wybierz użytkownika i specjalizację.", "OK");
                return;
            }

            // 2. Sprawdzenie flagi IsBusy (zapobiega wielokrotnemu kliknięciu)
            if (IsBusy) return;

            // 3. Rozpoczęcie operacji - ustawienie IsBusy i aktualizacja stanu komendy
            IsBusy = true;
            (SaveDoctorCommand as Command)?.ChangeCanExecute();

            DoctorForView resultDoctor = null;

            // 4. Blok try-finally zapewnia, że IsBusy zawsze zostanie zresetowane
            try
            {

                var createDto = new DoctorCreateDto
                {
                    UserId = SelectedUser.UserId,
                    SpecializationId = SelectedSpecialization.SpecializationId
                };

                try
                {
                    resultDoctor = await _doctorService.CreateDoctorAsync(createDto);

                }
                catch (ApiException apiEx) when (apiEx.StatusCode == 201)
                {

                    Console.WriteLine($"API returned 201 Created. Treating as success. Response: {apiEx.Response}");

                    resultDoctor = new DoctorForView();
                }

                if (resultDoctor != null) 
                {
                    await Application.Current.MainPage.DisplayAlert("Sukces", "Lekarz został dodany.", "OK");
                    
                    if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                    {
                        await navigationPage.Navigation.PopAsync();
                    }
                    else
                    {               
                        Console.WriteLine("Error navigating back: MainPage is not FlyoutPage or Detail is not NavigationPage.");                       
                    }
                }
                else
                {
                    // Ten blok obsłuży przypadek, gdy CreateDoctorAsync zwróci null bez wyjątku (jeśli API jest tak napisane)
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Nie udało się dodać lekarza (API nie zwróciło potwierdzenia).", "OK");
                }
            }
           
            finally 
            {
                // 5. Zakończenie operacji - reset IsBusy i aktualizacja stanu komendy
                IsBusy = false;
                (SaveDoctorCommand as Command)?.ChangeCanExecute();
            }
        }

        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsBusy))
            {
                (SaveDoctorCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}