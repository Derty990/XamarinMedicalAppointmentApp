// W Xamarin - Services/ClinicDataStore.cs
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient;
// using MedicalAppointmentApp.XamarinApp.Dtos; // Niepotrzebne, używamy wygenerowanych
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using MedicalAppointmentApp.Services.Abstract;

// T to WYGENEROWANA klasa ClinicForView
public class ClinicDataStore : AListDataStore<ClinicForView>, IClinicService
{
    private readonly MedicalApiClient _apiClient;
    public ClinicDataStore() { _apiClient = DependencyService.Get<MedicalApiClient>(); if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered."); }

    protected override async Task<List<ClinicForView>> GetItemsFromService()
    {
        // SPRAWDŹ NAZWĘ! np. ClinicsAllAsync
        ICollection<ClinicForView> result = await _apiClient.ClinicsAllAsync();
        return result?.ToList() ?? new List<ClinicForView>();
    }

    protected override Task<ClinicForView> GetItemFromService(int id)
    {
        // SPRAWDŹ NAZWĘ! np. ClinicsGETAsync
        return _apiClient.ClinicsGETAsync(id);
    }

    protected override Task<bool> DeleteItemFromService(int id)
    {
        return CallApiAndReturnBool(async () => await _apiClient.ClinicsDELETEAsync(id)); // SPRAWDŹ NAZWĘ!
    }

    public override ClinicForView Find(int id) => items?.FirstOrDefault(s => s.ClinicId == id); // Zakłada ClinicId

    // Metody Add/Update z bazy nie pasują
    protected override Task<ClinicForView> AddItemToService(ClinicForView item) => throw new NotImplementedException("Use CreateClinicAsync");
    protected override Task<bool> UpdateItemInService(ClinicForView item) => throw new NotImplementedException("Use UpdateClinicAsync");

    // Dedykowane metody
    public Task<ClinicForView> CreateClinicAsync(ClinicCreateDto createDto) // Używa WYGENEROWANEGO ClinicCreateDto
    {
        // SPRAWDŹ NAZWĘ! np. ClinicsPOSTAsync
        return _apiClient.ClinicsPOSTAsync(createDto);
    }
    public Task UpdateClinicAsync(int id, ClinicCreateDto updateDto) // Używa WYGENEROWANEGO ClinicCreateDto
    {
        // SPRAWDŹ NAZWĘ! np. ClinicsPUTAsync
        return _apiClient.ClinicsPUTAsync(id, updateDto);
    }

    // Metoda pomocnicza
    private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall) { try { await apiCall(); return true; } catch (ApiException ex) when (ex.StatusCode == 404) { return false; } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"API call failed: {ex.Message}"); return false; } }
}
