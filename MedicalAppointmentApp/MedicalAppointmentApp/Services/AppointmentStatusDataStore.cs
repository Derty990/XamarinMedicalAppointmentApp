// W Xamarin - Services/AppointmentStatusDataStore.cs
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

// T to WYGENEROWANY AppointmentStatusForView
public class AppointmentStatusDataStore : AListDataStore<AppointmentStatusForView>, IAppointmentStatusService
{
    private readonly MedicalApiClient _apiClient;
    public AppointmentStatusDataStore() { _apiClient = DependencyService.Get<MedicalApiClient>(); if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered."); }

    protected override async Task<List<AppointmentStatusForView>> GetItemsFromService()
    {
        // SPRAWDŹ NAZWĘ! np. AppointmentStatusesAllAsync
        ICollection<AppointmentStatusForView> result = await _apiClient.AppointmentStatusesAllAsync();
        return result?.ToList() ?? new List<AppointmentStatusForView>();
    }

    protected override Task<AppointmentStatusForView> GetItemFromService(int id)
    {
        // SPRAWDŹ NAZWĘ! np. AppointmentStatusesGETAsync
        return _apiClient.AppointmentStatusesGETAsync(id);
    }

    // Zakładamy, że API przyjmuje i zwraca TEN SAM typ (wygenerowany AppointmentStatusForView)
    protected override Task<AppointmentStatusForView> AddItemToService(AppointmentStatusForView item)
    {
        // SPRAWDŹ NAZWĘ! np. AppointmentStatusesPOSTAsync
        return _apiClient.AppointmentStatusesPOSTAsync(item);
    }

    protected override Task<bool> UpdateItemInService(AppointmentStatusForView item)
    {
        // SPRAWDŹ NAZWĘ! np. AppointmentStatusesPUTAsync
        // Zakłada, że wygenerowany AppointmentStatusForView ma StatusId
        return CallApiAndReturnBool(async () => await _apiClient.AppointmentStatusesPUTAsync(item.StatusId, item));
    }

    protected override Task<bool> DeleteItemFromService(int id)
    {
        // SPRAWDŹ NAZWĘ! np. AppointmentStatusesDELETEAsync
        return CallApiAndReturnBool(async () => await _apiClient.AppointmentStatusesDELETEAsync(id));
    }

    public override AppointmentStatusForView Find(int id)
    {
        // Zakłada, że wygenerowany AppointmentStatusForView ma StatusId
        return items?.FirstOrDefault(s => s.StatusId == id);
    }

    // Metoda pomocnicza
    private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall)
    {
        try { await apiCall(); return true; }
        catch (ApiException ex) when (ex.StatusCode == 404) { return false; }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"API call failed: {ex.Message}"); return false; }
    }
}