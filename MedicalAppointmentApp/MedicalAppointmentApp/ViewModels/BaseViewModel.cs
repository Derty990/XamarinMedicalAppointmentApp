// W Xamarin - np. ViewModels/BaseViewModel.cs
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MedicalAppointmentApp.XamarinApp.ViewModels // Użyj właściwego namespace
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Metoda pomocnicza do wywoływania zdarzenia PropertyChanged
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Metoda pomocnicza do ustawiania wartości pola i wywoływania OnPropertyChanged
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // Możesz tu dodać wspólne właściwości, np. IsBusy
        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        // Właściwość pomocnicza dla IsEnabled przycisków
        public bool IsNotBusy => !IsBusy;

        // Wywołaj OnPropertyChanged dla IsNotBusy, gdy IsBusy się zmienia
        protected virtual void OnIsBusyChanged() => OnPropertyChanged(nameof(IsNotBusy));

        // Przesłoń SetProperty dla IsBusy, aby wywołać OnIsBusyChanged
        protected bool SetBusy(ref bool backingStore, bool value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<bool>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            OnIsBusyChanged(); // Dodatkowe wywołanie dla IsNotBusy
            return true;
        }
        // Używaj SetBusy do ustawiania właściwości IsBusy:
        // public bool IsBusy { get => isBusy; set => SetBusy(ref isBusy, value); }
        // UWAGA: Trzeba by zmodyfikować SetProperty, aby wywoływało metodę wirtualną OnPropertyNameChanged
        // Dla uproszczenia można ręcznie wywołać OnPropertyChanged(nameof(IsNotBusy)) w setterze IsBusy
        // lub w komendzie po zmianie IsBusy. Zastosujemy to drugie podejście w RegisterViewModel.
    }
}