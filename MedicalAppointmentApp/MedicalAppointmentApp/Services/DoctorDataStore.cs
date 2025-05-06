using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using MedicalAppointmentApp.Services.Abstract;


public class DoctorDataStore : AListDataStore<DoctorForView>, IDoctorService
{
    private readonly MedicalApiClient _apiClient;

    public DoctorDataStore()
    {
        _apiClient = DependencyService.Get<MedicalApiClient>();
        if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered.");
    }

    protected override async Task<List<DoctorForView>> GetItemsFromService()
    {
        try
        {
           
            ICollection<DoctorForView> result = (ICollection<DoctorForView>)await _apiClient.DoctorsAllAsync();
            return result?.ToList() ?? new List<DoctorForView>();
        }
        catch (Exception ex) { Debug.WriteLine($"[DoctorDataStore] GetItemsFromService Error: {ex.Message}"); return null; }
    }

    protected override async Task<DoctorForView> GetItemFromService(int id)
    {
        try
        {
            return await _apiClient.DoctorsGETAsync(id);
        }
        catch (ApiException ex) when (ex.StatusCode == 404) { return null; }
        catch (Exception ex) { Debug.WriteLine($"[DoctorDataStore] GetItemFromService Error: {ex.Message}"); return null; }
    }
    protected override Task<bool> DeleteItemFromService(int id)
    {
        
        return CallApiAndReturnBool(async () => await _apiClient.DoctorsDELETEAsync(id));
    }  
    public override DoctorForView Find(int id)
    {
       
        return items?.FirstOrDefault(d => d.DoctorId == id);
    }
 
    protected override Task<DoctorForView> AddItemToService(DoctorForView item) => throw new NotImplementedException("Use CreateDoctorAsync instead.");
    protected override Task<bool> UpdateItemInService(DoctorForView item) => throw new NotImplementedException("Use UpdateDoctorSpecializationAsync instead.");

    // W pliku DoctorDataStore.cs

    public async Task<List<DoctorListItemDto>> GetDoctorListItemsAsync()
    {
        // Ta metoda powinna bezpośrednio wywołać endpoint API,
        // który zwraca DoctorListItemDto (czyli GET /api/doctors)
        // zamiast używać generycznego GetItemsAsync() dla DoctorForView.

        try
        {
            // Wywołaj metodę klienta API, która odpowiada GET /api/doctors
            // i zwraca kolekcję DoctorListItemDto.
            // Zakładamy, że nazywa się DoctorsAllAsync() i zwraca odpowiedni typ
            // (nawet jeśli GetItemsFromService próbowało rzutować inaczej).
            // Sprawdź dokładną nazwę i typ zwracany w MedicalApiClient.cs!
            ICollection<DoctorListItemDto> result = await _apiClient.DoctorsAllAsync(); // Zakładamy, że to zwraca ICollection<DoctorListItemDto>

            // Zwróć listę lub pustą listę, jeśli wynik jest null
            return result?.ToList() ?? new List<DoctorListItemDto>();
        }
        catch (ApiException apiEx) // Obsłuż błędy API specyficznie
        {
            Debug.WriteLine($"[DoctorDataStore] API Error getting doctor list items: {apiEx.StatusCode} - {apiEx.Response}");
            // Możesz tu dodać logikę specyficzną dla błędów API
            return new List<DoctorListItemDto>(); // Zwróć pustą listę w razie błędu API
        }
        catch (Exception ex) // Obsłuż inne błędy
        {
            Debug.WriteLine($"[DoctorDataStore] Error in GetDoctorListItemsAsync: {ex.Message}");
            return new List<DoctorListItemDto>(); // Zwróć pustą listę w razie innego błędu
        }
    }

    // --- UWAGA: Metody z AListDataStore<DoctorForView> poniżej nadal działają ---
    // --- w kontekście pobierania/zarządzania PEŁNYM DoctorForView,      ---
    // --- np. dla strony szczegółów/edycji, jeśli GetItemFromService działa poprawnie ---
    // --- dla GET /api/doctors/{id} zwracającego DoctorForView.             ---

    // protected override async Task<List<DoctorForView>> GetItemsFromService() { ... } // Ta metoda może być teraz nieużywana lub błędna
    // protected override async Task<DoctorForView> GetItemFromService(int id) { ... } // Ta jest prawdopodobnie OK dla GetDoctor(id)
    // ... reszta metod DataStore ...

    public Task<DoctorForView> CreateDoctorAsync(DoctorCreateDto createDto)
    {
      
        return _apiClient.DoctorsPOSTAsync(createDto);
        
    }

    public Task UpdateDoctorSpecializationAsync(int id, DoctorUpdateDto updateDto)
    {
       
        return _apiClient.DoctorsPUTAsync(id, updateDto);
       
    }

    private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall)
    {
        try { await apiCall(); return true; }
        catch (ApiException ex) when (ex.StatusCode == 404) { return false; }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"API call failed: {ex.Message}"); return false; }
    }
}

