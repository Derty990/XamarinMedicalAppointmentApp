// W Xamarin - Services/AppointmentStatusDataStore.cs
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

public class AppointmentStatusDataStore : AListDataStore<AppointmentStatusForView>, IAppointmentStatusService
{
    private readonly MedicalApiClient _apiClient;
    public AppointmentStatusDataStore() { _apiClient = DependencyService.Get<MedicalApiClient>(); if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered."); }

    protected override async Task<List<AppointmentStatusForView>> GetItemsFromService()
    {
       
        ICollection<AppointmentStatusForView> result = await _apiClient.AppointmentStatusesAllAsync();
        return result?.ToList() ?? new List<AppointmentStatusForView>();
    }

    protected override Task<AppointmentStatusForView> GetItemFromService(int id)
    {
       
        return _apiClient.AppointmentStatusesGETAsync(id);
    }

   
    protected override Task<AppointmentStatusForView> AddItemToService(AppointmentStatusForView item)
    {
       
        return _apiClient.AppointmentStatusesPOSTAsync(item);
    }

    protected override Task<bool> UpdateItemInService(AppointmentStatusForView item)
    {
       
        return CallApiAndReturnBool(async () => await _apiClient.AppointmentStatusesPUTAsync(item.StatusId, item));
    }

    protected override Task<bool> DeleteItemFromService(int id)
    {
        
        return CallApiAndReturnBool(async () => await _apiClient.AppointmentStatusesDELETEAsync(id));
    }

    public override AppointmentStatusForView Find(int id)
    {
        
        return items?.FirstOrDefault(s => s.StatusId == id);
    }

    private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall)
    {
        try { await apiCall(); return true; }
        catch (ApiException ex) when (ex.StatusCode == 404) { return false; }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"API call failed: {ex.Message}"); return false; }
    }
}