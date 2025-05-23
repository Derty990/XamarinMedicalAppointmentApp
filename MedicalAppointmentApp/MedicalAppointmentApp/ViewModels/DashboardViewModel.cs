﻿using MedicalAppointmentApp.Views; 
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MedicalAppointmentApp.XamarinApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel 
    {
        private string _welcomeMessage;
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        private string _lastAppointmentInfo;
        public string LastAppointmentInfo
        {
            get => _lastAppointmentInfo;
            set => SetProperty(ref _lastAppointmentInfo, value);
        }
        public string Title { get; }

        public ICommand NavigateCommand { get; }
        public DashboardViewModel()
        {
            Title = "Pulpit"; 
            NavigateCommand = new Command<string>(async (routeName) => await ExecuteNavigateCommand(routeName), (routeName) => !IsBusy); // Dodano CanExecute
            LoadWelcomeData();
        }

        private void LoadWelcomeData()
        {
            string firstName = "Użytkowniku"; 
            if (Application.Current.Properties.ContainsKey("LoggedInUserFirstName"))
            {
                firstName = Application.Current.Properties["LoggedInUserFirstName"] as string ?? "Użytkowniku";
            }
            WelcomeMessage = $"Witaj, {firstName}";

            LastAppointmentInfo = "Brak nadchodzących wizyt";
            // TODO: W przyszłości zaimplementować ładowanie wizyt z serwisu
        }

        private async Task ExecuteNavigateCommand(string routeName)
        {
            if (IsBusy || string.IsNullOrEmpty(routeName))
                return;

            Page pageToNavigate = null;
            try
            {

                switch (routeName)
                {
                    // Odkomentuj, gdy będą odpowiednie strony:
                     case nameof(DoctorsPage):
                         pageToNavigate = new DoctorsPage();
                         break;
                     //case nameof(AppointmentBookingPage):
                      //   pageToNavigate = new AppointmentBookingPage();
                       //  break;
                    // case nameof(MyAppointmentsPage):
                     //    pageToNavigate = new MyAppointmentsPage();
                      //   break;
                     case nameof(ProfilePage):
                         pageToNavigate = new ProfilePage();
                         break;
                    default:
                        Console.WriteLine($"Nieznana lub jeszcze niezaimplementowana trasa: {routeName}");
                        await Application.Current.MainPage.DisplayAlert("Nawigacja", $"Strona '{routeName}' nie jest jeszcze dostępna.", "OK");
                        return;
                }

                // Nawigacja za pomocą stosu NavigationPage w Detail
                if (pageToNavigate != null)
                {
                    IsBusy = true;
                    (NavigateCommand as Command)?.ChangeCanExecute();

                    
                    if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage navigationPage)
                    {
                        await navigationPage.Navigation.PushAsync(pageToNavigate);
                    }
                    else
                    {
                        Debug.WriteLine("Error: MainPage is not FlyoutPage or Detail is not NavigationPage");
                        await Application.Current.MainPage.DisplayAlert("Błąd", "Wystąpił wewnętrzny błąd nawigacji.", "OK");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Nawigacja do {routeName} nie powiodła się: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Błąd nawigacji", $"Nie można przejść do: {routeName}", "OK");
            }
            finally
            {
                IsBusy = false;
                (NavigateCommand as Command)?.ChangeCanExecute();
            }
        }
        public void OnAppearing()
        {
            Console.WriteLine("Dashboard ViewModel Appearing - reloading welcome data.");
            LoadWelcomeData();
            // TODO: Dodaj odświeżanie innych danych, np. listy wizyt
        }
       
        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsBusy))
            {
                (NavigateCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}