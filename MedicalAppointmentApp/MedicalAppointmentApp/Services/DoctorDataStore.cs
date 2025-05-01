// W Xamarin - Services/DoctorDataStore.cs
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient; // Namespace WYGENEROWANEGO kodu
// using MedicalAppointmentApp.XamarinApp.Dtos; // Niepotrzebne, jeśli DTO są generowane
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using MedicalAppointmentApp.Services.Abstract;

// T w AListDataStore to WYGENEROWANY DoctorForView (pełne dane)
public class DoctorDataStore : AListDataStore<DoctorForView>, IDoctorService
{
    private readonly MedicalApiClient _apiClient;

    public DoctorDataStore()
    {
        _apiClient = DependencyService.Get<MedicalApiClient>();
        if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered.");
    }

    // --- Implementacja metod abstrakcyjnych z AListDataStore<DoctorForView> ---

    // Pobiera listę PEŁNYCH danych lekarzy z API
    protected override async Task<List<DoctorForView>> GetItemsFromService()
    {
        try
        {
            // SPRAWDŹ NAZWĘ! Powinna zwracać ICollection<DoctorForView>
            ICollection<DoctorForView> result = (ICollection<DoctorForView>)await _apiClient.DoctorsAllAsync();
            return result?.ToList() ?? new List<DoctorForView>();
        }
        catch (Exception ex) { Debug.WriteLine($"[DoctorDataStore] GetItemsFromService Error: {ex.Message}"); return null; }
    }

    // Pobiera PEŁNE dane jednego lekarza z API
    protected override async Task<DoctorForView> GetItemFromService(int id)
    {
        try
        {
            // SPRAWDŹ NAZWĘ! Powinna zwracać DoctorForView
            return await _apiClient.DoctorsGETAsync(id);
        }
        catch (ApiException ex) when (ex.StatusCode == 404) { return null; }
        catch (Exception ex) { Debug.WriteLine($"[DoctorDataStore] GetItemFromService Error: {ex.Message}"); return null; }
    }

    // Usuwa lekarza przez API
    protected override Task<bool> DeleteItemFromService(int id)
    {
        // SPRAWDŹ NAZWĘ! Zwraca Task<bool> przez metodę pomocniczą
        return CallApiAndReturnBool(async () => await _apiClient.DoctorsDELETEAsync(id));
    }

    // Implementacja Find dla cache 'items' (operuje na DoctorForView)
    public override DoctorForView Find(int id)
    {
        // Zakładamy, że WYGENEROWANE DoctorForView ma właściwość DoctorId
        return items?.FirstOrDefault(d => d.DoctorId == id);
    }

    // Te metody z AListDataStore nie są używane bezpośrednio dla Doctor
    protected override Task<DoctorForView> AddItemToService(DoctorForView item) => throw new NotImplementedException("Use CreateDoctorAsync instead.");
    protected override Task<bool> UpdateItemInService(DoctorForView item) => throw new NotImplementedException("Use UpdateDoctorSpecializationAsync instead.");

    // --- Implementacja dedykowanych metod z interfejsu IDoctorService ---

    // Pobiera listę UPROSZCZONĄ (mapowanie w Xamarin)
    public async Task<List<DoctorListItemDto>> GetDoctorListItemsAsync()
    {
        // Pobierz pełne dane (z cache'u lub API przez metodę bazową GetItemsAsync)
        var fullDoctors = await GetItemsAsync(false); // false = użyj cache jeśli jest

        if (fullDoctors == null) return new List<DoctorListItemDto>(); // Zwróć pustą listę w razie błędu

        // Ręczne mapowanie z DoctorForView na DoctorListItemDto
        return fullDoctors.Select(d => new DoctorListItemDto
        {
            DoctorId = d.DoctorId,
            // Zakładamy, że DoctorForView ma FirstName i LastName (spłaszczone z User)
            FullName = $"{d.FirstName} {d.LastName}".Trim(),
            // Zakładamy, że DoctorForView ma SpecializationName
            SpecializationName = d.SpecializationName ?? "N/A"
        }).ToList();
    }


    // Tworzy rekord Doctor używając WYGENEROWANEGO DoctorCreateDto
    public Task<DoctorForView> CreateDoctorAsync(DoctorCreateDto createDto)
    {
        // SPRAWDŹ NAZWĘ! Powinna przyjmować DoctorCreateDto i zwracać DoctorForView
        return _apiClient.DoctorsPOSTAsync(createDto);
        // Wyjątki polecą do ViewModelu
    }

    // Aktualizuje specjalizację używając WYGENEROWANEGO DoctorUpdateDto
    public Task UpdateDoctorSpecializationAsync(int id, DoctorUpdateDto updateDto)
    {
        // SPRAWDŹ NAZWĘ! Powinna przyjmować DoctorUpdateDto i zwracać Task
        return _apiClient.DoctorsPUTAsync(id, updateDto);
        // Wyjątki polecą do ViewModelu
    }


    // Metoda pomocnicza (może być w klasie bazowej lub helperze)
    private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall)
    {
        try { await apiCall(); return true; }
        catch (ApiException ex) when (ex.StatusCode == 404) { return false; }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"API call failed: {ex.Message}"); return false; }
    }
}

