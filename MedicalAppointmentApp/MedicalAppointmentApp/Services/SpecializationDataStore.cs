// W Xamarin - Services/SpecializationDataStore.cs
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient; // Namespace WYGENEROWANEGO kodu
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics; // Dla Debug.WriteLine

namespace MedicalAppointmentApp.XamarinApp.Services // Użyj właściwego namespace
{
    // T w AListDataStore to WYGENEROWANA klasa SpecializationForView
    public class SpecializationDataStore : AListDataStore<SpecializationForView>, IDataStore<SpecializationForView>, ISpecializationService 
    {
        private readonly MedicalApiClient _apiClient; // WYGENEROWANY Klient

        public SpecializationDataStore()
        {
            _apiClient = DependencyService.Get<MedicalApiClient>();
            if (_apiClient == null)
            {
                throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered with DependencyService.");
            }
        }

        // --- Implementacja metod abstrakcyjnych z AListDataStore ---

        // Pobiera listę specjalizacji z API
        protected override async Task<List<SpecializationForView>> GetItemsFromService()
        {
            // Wywołaj metodę z WYGENEROWANEGO klienta (_apiClient)
            // SPRAWDŹ DOKŁADNĄ NAZWĘ METODY! np. SpecializationsAllAsync
            ICollection<SpecializationForView> specializations = await _apiClient.SpecializationsAllAsync();
            return specializations?.ToList() ?? new List<SpecializationForView>();
            // Ewentualny wyjątek poleci wyżej do ViewModelu
        }

        // Pobiera jedną specjalizację z API
        protected override Task<SpecializationForView> GetItemFromService(int id)
        {
            // Wywołaj metodę z WYGENEROWANEGO klienta (_apiClient)
            // SPRAWDŹ DOKŁADNĄ NAZWĘ METODY! np. SpecializationsGETAsync
            return _apiClient.SpecializationsGETAsync(id);
            // Ewentualny wyjątek (w tym 404) poleci wyżej
        }

        // Dodaje nową specjalizację przez API
        // Zakładamy, że metoda POST API przyjmuje i zwraca ten sam typ SpecializationForView
        protected override Task<SpecializationForView> AddItemToService(SpecializationForView item)
        {
            // SPRAWDŹ DOKŁADNĄ NAZWĘ METODY! np. SpecializationsPOSTAsync
            // Sprawdź, czy przyjmuje i zwraca wygenerowany SpecializationForView
            return _apiClient.SpecializationsPOSTAsync(item);
            // Ewentualny wyjątek poleci wyżej
        }

        // Aktualizuje specjalizację przez API
        // Zakładamy, że metoda PUT API przyjmuje ten sam typ SpecializationForView
        protected override Task<bool> UpdateItemInService(SpecializationForView item)
        {
            // SPRAWDŹ DOKŁADNĄ NAZWĘ METODY! np. SpecializationsPUTAsync
            // Zakładamy, że wygenerowany SpecializationForView ma właściwość SpecializationId
            // Używamy metody pomocniczej do opakowania wywołania API
            return CallApiAndReturnBool(async () => await _apiClient.SpecializationsPUTAsync(item.SpecializationId, item));
        }

        // Usuwa specjalizację przez API
        protected override Task<bool> DeleteItemFromService(int id)
        {
            // SPRAWDŹ DOKŁADNĄ NAZWĘ METODY! np. SpecializationsDELETEAsync
            // Używamy metody pomocniczej do opakowania wywołania API
            return CallApiAndReturnBool(async () => await _apiClient.SpecializationsDELETEAsync(id));
        }

        // Implementacja metody Find dla cache 'items'
        public override SpecializationForView Find(int id)
        {
            // Zakładamy, że WYGENEROWANA klasa SpecializationForView ma właściwość SpecializationId
            return items?.FirstOrDefault(s => s.SpecializationId == id);
        }

        // --- Koniec Implementacji metod abstrakcyjnych ---

        // Metoda pomocnicza opakowująca wywołanie API zwracające Task w Task<bool>
        // Można ją przenieść do klasy bazowej AListDataStore lub wspólnego helpera
        private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall)
        {
            await apiCall(); // Pozwól wyjątkom lecieć wyżej
            return true; // Zwróć true, jeśli nie było wyjątku
                         // Obsługa błędów (np. ApiException 404) musi być w ViewModelu
        }
    }
}