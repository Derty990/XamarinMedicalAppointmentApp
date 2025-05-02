using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using MedicalAppointmentApp.Services.Abstract;


public class ClinicDataStore : AListDataStore<ClinicForView>, IClinicService
{
    private readonly MedicalApiClient _apiClient;
    public ClinicDataStore() { _apiClient = DependencyService.Get<MedicalApiClient>(); if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered."); }

    protected override async Task<List<ClinicForView>> GetItemsFromService()
    {
       
        ICollection<ClinicForView> result = await _apiClient.ClinicsAllAsync();
        return result?.ToList() ?? new List<ClinicForView>();
    }

    protected override Task<ClinicForView> GetItemFromService(int id)
    {
       
        return _apiClient.ClinicsGETAsync(id);
    }

    protected override Task<bool> DeleteItemFromService(int id)
    {
        return CallApiAndReturnBool(async () => await _apiClient.ClinicsDELETEAsync(id)); // SPRAWDŹ NAZWĘ!
    }

    public override ClinicForView Find(int id) => items?.FirstOrDefault(s => s.ClinicId == id); // Zakłada ClinicId

    
    protected override Task<ClinicForView> AddItemToService(ClinicForView item) => throw new NotImplementedException("Use CreateClinicAsync");
    protected override Task<bool> UpdateItemInService(ClinicForView item) => throw new NotImplementedException("Use UpdateClinicAsync");


    public Task<ClinicForView> CreateClinicAsync(ClinicCreateDto createDto) // Używa WYGENEROWANEGO ClinicCreateDto
    {
       
        return _apiClient.ClinicsPOSTAsync(createDto);
    }
    public Task UpdateClinicAsync(int id, ClinicCreateDto updateDto) // Używa WYGENEROWANEGO ClinicCreateDto
    {
     
        return _apiClient.ClinicsPUTAsync(id, updateDto);
    }

   
    private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall) { try { await apiCall(); return true; } catch (ApiException ex) when (ex.StatusCode == 404) { return false; } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"API call failed: {ex.Message}"); return false; } }
}
