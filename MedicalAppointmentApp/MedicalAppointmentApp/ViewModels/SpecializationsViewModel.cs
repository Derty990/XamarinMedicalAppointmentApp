using MedicalAppointmentApp.Services.Abstract;
using MedicalAppointmentApp.Views;            
using MedicalAppointmentApp.XamarinApp.ApiClient;
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class SpecializationsViewModel : BaseViewModel
    {
        public string Title { get; }

        private readonly ISpecializationService _specializationService;

        public ObservableCollection<SpecializationForView> Specializations { get; }
        public ICommand LoadSpecializationsCommand { get; }
        public ICommand AddSpecializationCommand { get; }
        public ICommand EditSpecializationCommand { get; } 
        // Można dodać DeleteCommand później
        private SpecializationForView _selectedSpecialization;
        public SpecializationForView SelectedSpecialization
        {
            get => _selectedSpecialization;
            set
            {
                if (SetProperty(ref _selectedSpecialization, value) && value != null)
                {
                    // Automatycznie wywołaj edycję po wybraniu
                    EditSpecializationCommand.Execute(value);
                }
            }
        }

        public SpecializationsViewModel()
        {
            Title = "Specjalizacje";
            _specializationService = DependencyService.Get<ISpecializationService>();
            if (_specializationService == null) { Console.WriteLine("BŁĄD: Nie zarejestrowano ISpecializationService!"); }

            Specializations = new ObservableCollection<SpecializationForView>();

            LoadSpecializationsCommand = new Command(async () => await ExecuteLoadSpecializationsCommand(), () => !IsBusy);
            AddSpecializationCommand = new Command(async () => await ExecuteAddSpecializationCommand(), () => !IsBusy);
            EditSpecializationCommand = new Command<SpecializationForView>(async (spec) => await ExecuteEditSpecializationCommand(spec), (spec) => !IsBusy && spec != null);
        }

        async Task ExecuteLoadSpecializationsCommand()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                Specializations.Clear();
                var items = await _specializationService.GetItemsAsync(true); 
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Specializations.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania specjalizacji: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można załadować listy specjalizacji.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteAddSpecializationCommand()
        {
            if (IsBusy) return;
            try
            {
                if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                {
                    
                    await navigationPage.Navigation.PushAsync(new AddEditSpecializationPage());
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Nawigacja do AddEditSpecializationPage nie powiodła się: {ex.Message}"); }
        }

        async Task ExecuteEditSpecializationCommand(SpecializationForView specialization)
        {
            if (specialization == null || IsBusy)
                return;

            try
            {
                Console.WriteLine($"Nawigacja do AddEditSpecializationPage dla Specialization ID: {specialization.SpecializationId}");

                if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                {
                    await navigationPage.Navigation.PushAsync(new AddEditSpecializationPage(specialization.SpecializationId));
                }
                else
                {
                    Debug.WriteLine("Błąd nawigacji do edycji specjalizacji: Struktura MainPage jest niepoprawna.");
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Wystąpił wewnętrzny błąd nawigacji.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Nawigacja do AddEditSpecializationPage (Edit) nie powiodła się: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie można otworzyć strony edycji specjalizacji.", "OK");
            }
            finally
            {
                SelectedSpecialization = null;
            }
        }
        // Metoda do wywołania z OnAppearing strony
        public void OnAppearing()
        {
            IsBusy = false; // Resetuj flagę
            SelectedSpecialization = null; // Resetuj zaznaczenie
            // Załaduj/odśwież dane przy każdym pojawieniu się strony
            LoadSpecializationsCommand.Execute(null);
        }

        // Aktualizacja CanExecute komend
        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsBusy))
            {
                (LoadSpecializationsCommand as Command)?.ChangeCanExecute();
                (AddSpecializationCommand as Command)?.ChangeCanExecute();
                (EditSpecializationCommand as Command)?.ChangeCanExecute();
            }
            else if (propertyName == nameof(SelectedSpecialization))
            {
                (EditSpecializationCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}