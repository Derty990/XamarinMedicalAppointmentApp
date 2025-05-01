// W Xamarin - Services/AppointmentDataStore.cs
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient; // Namespace WYGENEROWANEGO kodu
// using MedicalAppointmentApp.XamarinApp.Dtos; // Niepotrzebne, DTO są generowane
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using MedicalAppointmentApp.Services.Abstract;

namespace MedicalAppointmentApp.XamarinApp.Services // Użyj właściwego namespace
{
    // T w AListDataStore to WYGENEROWANY AppointmentForView
    public class AppointmentDataStore : AListDataStore<AppointmentForView>, IAppointmentService
    {
        private readonly MedicalApiClient _apiClient; // WYGENEROWANY Klient

        public AppointmentDataStore()
        {
            _apiClient = DependencyService.Get<MedicalApiClient>();
            if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered.");
        }

        // --- Implementacja metod abstrakcyjnych z AListDataStore ---

        protected override async Task<List<AppointmentForView>> GetItemsFromService()
        {
            try {
                // SPRAWDŹ NAZWĘ METODY! np. AppointmentsAllAsync
                ICollection<AppointmentForView> result = await _apiClient.AppointmentsAllAsync();
                return result?.ToList() ?? new List<AppointmentForView>();
            }
            catch (Exception ex) { Debug.WriteLine($"[AppointmentDataStore] GetItemsFromService Error: {ex.Message}"); return null; }
        }

        protected override async Task<AppointmentForView> GetItemFromService(int id)
        {
            try {
                // SPRAWDŹ NAZWĘ METODY! np. AppointmentsGETAsync
                return await _apiClient.AppointmentsGETAsync(id);
            }
            catch (ApiException ex) when (ex.StatusCode == 404) { return null; }
            catch (Exception ex) { Debug.WriteLine($"[AppointmentDataStore] GetItemFromService Error: {ex.Message}"); return null; }
        }

        // Implementacja metody usuwającej dane przez API
        protected override Task<bool> DeleteItemFromService(int id)
        {
            // SPRAWDŹ NAZWĘ METODY! np. AppointmentsDELETEAsync
            // Używamy metody pomocniczej, aby zwrócić bool, łapiąc wyjątki
            return CallApiAndReturnBool(async () => await _apiClient.AppointmentsDELETEAsync(id));
        }

        // Implementacja metody Find dla cache 'items'
        public override AppointmentForView Find(int id)
        {
            // Zakładamy, że WYGENEROWANA klasa AppointmentForView ma właściwość AppointmentId
            return items?.FirstOrDefault(a => a.AppointmentId == id);
        }

        // Te metody generyczne nie pasują - rzucamy wyjątek
        protected override Task<AppointmentForView> AddItemToService(AppointmentForView item) => throw new NotImplementedException("Use CreateAppointmentAsync instead.");
        protected override Task<bool> UpdateItemInService(AppointmentForView item) => throw new NotImplementedException("Use UpdateAppointmentAsync instead.");

        // --- Implementacja dedykowanych metod z interfejsu IAppointmentService ---

        // Używa WYGENEROWANEGO AppointmentCreateDto i zwraca WYGENEROWANE AppointmentForView
        public Task<AppointmentForView> CreateAppointmentAsync(AppointmentCreateDto createDto)
        {
            // SPRAWDŹ NAZWĘ METODY! np. AppointmentsPOSTAsync
            return _apiClient.AppointmentsPOSTAsync(createDto);
            // Wyjątki (błędy API) polecą do ViewModelu
        }

        // Używa WYGENEROWANEGO AppointmentUpdateDto i zwraca Task
        public Task UpdateAppointmentAsync(int id, AppointmentUpdateDto updateDto)
        {
            // SPRAWDŹ NAZWĘ METODY! np. AppointmentsPUTAsync
            return _apiClient.AppointmentsPUTAsync(id, updateDto);
            // Wyjątki polecą do ViewModelu
        }

        // UWAGA: Usunęliśmy stąd metodę 'public override async Task<bool> DeleteItemAsync(int id)'
        // Ponieważ jest ona już zaimplementowana jako 'virtual' w klasie bazowej AListDataStore!

        // Metoda pomocnicza (może być w klasie bazowej lub helperze)
        private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall)
        {
            try { await apiCall(); return true; }
            catch (ApiException ex) when (ex.StatusCode == 404) { return false; }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"API call failed: {ex.Message}"); return false; }
        }
    }

}
