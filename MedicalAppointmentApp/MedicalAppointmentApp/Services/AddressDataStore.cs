// W Xamarin - Services/AddressDataStore.cs
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient; // Namespace WYGENEROWANEGO kodu
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using MedicalAppointmentApp.Services.Abstract;

// Używamy WYGENEROWANEGO AddressForView jako T
public class AddressDataStore : AListDataStore<AddressForView>, IAddressService
{
    private readonly MedicalApiClient _apiClient;

    public AddressDataStore()
    {
        _apiClient = DependencyService.Get<MedicalApiClient>();
        if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered.");
    }

    protected override async Task<List<AddressForView>> GetItemsFromService()
    {
        
        ICollection<AddressForView> result = await _apiClient.AddressesAllAsync();
        return result?.ToList() ?? new List<AddressForView>();
    }

    protected override Task<AddressForView> GetItemFromService(int id)
    {
        
        return _apiClient.AddressesGETAsync(id);
    }

    protected override Task<bool> DeleteItemFromService(int id)
    {
       
        return CallApiAndReturnBool(async () => await _apiClient.AddressesDELETEAsync(id));
    }

    public override AddressForView Find(int id)
    {
       
        return items?.FirstOrDefault(a => a.AddressId == id);
    }

    
    protected override Task<AddressForView> AddItemToService(AddressForView item) => throw new NotImplementedException("Use CreateAddressAsync");
    protected override Task<bool> UpdateItemInService(AddressForView item) => throw new NotImplementedException("Use UpdateAddressAsync");

   
    public Task<AddressForView> CreateAddressAsync(AddressCreateDto createDto)
    {
        
        return _apiClient.AddressesPOSTAsync(createDto);
    }

    public Task UpdateAddressAsync(int id, AddressCreateDto updateDto) 
    {
        
        return _apiClient.AddressesPUTAsync(id, updateDto);
    }

    
    private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall)
    {
        try { await apiCall(); return true; }
        catch (ApiException ex) when (ex.StatusCode == 404) { return false; }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"API call failed: {ex.Message}"); return false; }
    }
}
