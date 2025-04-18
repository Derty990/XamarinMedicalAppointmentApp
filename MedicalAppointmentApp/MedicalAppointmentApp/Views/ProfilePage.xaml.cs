﻿using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Initializing ProfilePage");
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("ProfilePage initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing ProfilePage: {ex}");
            }
           
        }

        async void OnEditProfileClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Edycja profilu", "Funkcja edycji profilu będzie dostępna wkrótce", "OK");
        }

        async void OnViewMedicalHistoryClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Historia medyczna", "Pełna historia medyczna będzie dostępna wkrótce", "OK");
        }

        async void OnViewPrescriptionsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Recepty", "Pełna lista recept będzie dostępna wkrótce", "OK");
        }

        async void OnChangePasswordClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Zmiana hasła", "Funkcja zmiany hasła będzie dostępna wkrótce", "OK");
        }

        async void OnNotificationsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Powiadomienia", "Ustawienia powiadomień będą dostępne wkrótce", "OK");
        }

        async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirmed = await DisplayAlert("Wylogowywanie",
                "Czy na pewno chcesz się wylogować?",
                "Tak", "Nie");

            if (confirmed)
            {
                // Powrót do strony logowania
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}