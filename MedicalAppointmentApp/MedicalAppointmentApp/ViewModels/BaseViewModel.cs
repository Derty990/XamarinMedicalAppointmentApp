using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MedicalAppointmentApp.XamarinApp.ViewModels 
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

       
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

       
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

       
        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        
        public bool IsNotBusy => !IsBusy;

       
        protected virtual void OnIsBusyChanged() => OnPropertyChanged(nameof(IsNotBusy));

        
        protected bool SetBusy(ref bool backingStore, bool value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<bool>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            OnIsBusyChanged(); 
            return true;
        }
       
    }
}