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
    protected override async Task DeleteItemFromService(int id) 
    {
        await _apiClient.DoctorsDELETEAsync(id, System.Threading.CancellationToken.None);
    }
    public override DoctorForView Find(int id)
    {
       
        return items?.FirstOrDefault(d => d.DoctorId == id);
    }
 
    protected override Task<DoctorForView> AddItemToService(DoctorForView item) => throw new NotImplementedException("Use CreateDoctorAsync instead.");
    protected override async Task UpdateItemInService(DoctorForView item) 
    {
        throw new NotImplementedException("Use UpdateDoctorSpecializationAsync in IDoctorService instead.");
    }

    public async Task<List<DoctorListItemDto>> GetDoctorListItemsAsync()
    {
        try
        {
            ICollection<DoctorListItemDto> result = await _apiClient.DoctorsAllAsync(); 
            return result?.ToList() ?? new List<DoctorListItemDto>();
        }
        catch (ApiException apiEx) 
        {
            Debug.WriteLine($"[DoctorDataStore] API Error getting doctor list items: {apiEx.StatusCode} - {apiEx.Response}");
            return new List<DoctorListItemDto>();
        }
        catch (Exception ex) 
        {
            Debug.WriteLine($"[DoctorDataStore] Error in GetDoctorListItemsAsync: {ex.Message}");
            return new List<DoctorListItemDto>(); 
        }
    }

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

