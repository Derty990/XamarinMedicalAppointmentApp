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

    public async Task<List<DoctorListItemDto>> GetDoctorListItemsAsync()
    {
     
        var fullDoctors = await GetItemsAsync(false); 

        if (fullDoctors == null) return new List<DoctorListItemDto>();

        
        return fullDoctors.Select(d => new DoctorListItemDto
        {
            DoctorId = d.DoctorId,
           
            FullName = $"{d.FirstName} {d.LastName}".Trim(),
           
            SpecializationName = d.SpecializationName ?? "N/A"
        }).ToList();
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

