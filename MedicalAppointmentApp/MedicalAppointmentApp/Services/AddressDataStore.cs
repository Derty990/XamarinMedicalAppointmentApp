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
        // SPRAWDŹ NAZWĘ! np. AddressesAllAsync, zwraca ICollection<AddressForView>
        ICollection<AddressForView> result = await _apiClient.AddressesAllAsync();
        return result?.ToList() ?? new List<AddressForView>();
    }

    protected override Task<AddressForView> GetItemFromService(int id)
    {
        // SPRAWDŹ NAZWĘ! np. AddressesGETAsync, zwraca Task<AddressForView>
        return _apiClient.AddressesGETAsync(id);
    }

    protected override Task<bool> DeleteItemFromService(int id)
    {
        // SPRAWDŹ NAZWĘ! np. AddressesDELETEAsync, zwraca Task
        return CallApiAndReturnBool(async () => await _apiClient.AddressesDELETEAsync(id));
    }

    public override AddressForView Find(int id)
    {
        // Zakładamy, że WYGENEROWANE AddressForView ma właściwość AddressId
        return items?.FirstOrDefault(a => a.AddressId == id);
    }

    // Metody Add/Update z bazy nie pasują
    protected override Task<AddressForView> AddItemToService(AddressForView item) => throw new NotImplementedException("Use CreateAddressAsync");
    protected override Task<bool> UpdateItemInService(AddressForView item) => throw new NotImplementedException("Use UpdateAddressAsync");

    // Dedykowane metody używające WYGENEROWANYCH DTO wejściowych
    public Task<AddressForView> CreateAddressAsync(AddressCreateDto createDto) // WYGENEROWANE DTO
    {
        // SPRAWDŹ NAZWĘ! np. AddressesPOSTAsync, czy przyjmuje AddressCreateDto i zwraca AddressForView
        return _apiClient.AddressesPOSTAsync(createDto);
    }

    public Task UpdateAddressAsync(int id, AddressCreateDto updateDto) // WYGENEROWANE DTO
    {
        // SPRAWDŹ NAZWĘ! np. AddressesPUTAsync, czy przyjmuje AddressCreateDto i zwraca Task
        return _apiClient.AddressesPUTAsync(id, updateDto);
    }

    // Metoda pomocnicza (może być w klasie bazowej lub helperze)
    private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall)
    {
        try { await apiCall(); return true; }
        catch (ApiException ex) when (ex.StatusCode == 404) { return false; }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"API call failed: {ex.Message}"); return false; }
    }
}
