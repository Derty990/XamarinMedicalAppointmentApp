using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading; 
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using MedicalAppointmentApp.Services.Abstract; 

namespace MedicalAppointmentApp.XamarinApp.Services
{
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
            try
            {
                ICollection<AddressForView> result = await _apiClient.AddressesAllAsync(CancellationToken.None);
                return result?.ToList() ?? new List<AddressForView>();
            }
            catch (Exception ex) { Debug.WriteLine($"[AddressDataStore] GetItems Error: {ex.Message}"); return null; }
        }

        protected override Task<AddressForView> GetItemFromService(int id)
        {
            return _apiClient.AddressesGETAsync(id, CancellationToken.None);
        } 

        protected override async Task DeleteItemFromService(int id) 
        {
         
            await _apiClient.AddressesDELETEAsync(id, System.Threading.CancellationToken.None);
           
        }

        protected override Task<AddressForView> AddItemToService(AddressForView item) => throw new NotImplementedException("Use CreateAddressAsync");
        protected override async Task UpdateItemInService(AddressForView item)
        {
            var updateDto = new AddressCreateDto 
            {
                Street = item.Street,
                City = item.City,
                PostalCode = item.PostalCode
            };
            
            await _apiClient.AddressesPUTAsync(item.AddressId, updateDto, CancellationToken.None);
          
        }
        public Task<AddressForView> CreateAddressAsync(AddressCreateDto createDto)
        {
            return _apiClient.AddressesPOSTAsync(createDto, CancellationToken.None); 
        }

        public Task UpdateAddressAsync(int id, AddressCreateDto updateDto)
        {
            return _apiClient.AddressesPUTAsync(id, updateDto, CancellationToken.None); 
        }


        public override AddressForView Find(int id)
        {
            return items?.FirstOrDefault(a => a.AddressId == id);
        }

      
        private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall)
        {
            try
            {
                await apiCall();
                return true; 
            }
            catch (ApiException apiEx) when (apiEx.StatusCode == 404) 
            {
                Debug.WriteLine($"[DataStore] API Info: Resource not found (404).");
                return false; 
            }
            catch (ApiException apiEx)
            {
                Debug.WriteLine($"[DataStore] API Error: {apiEx.StatusCode} - {apiEx.Response}");
                return false;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"[DataStore] General Error: {ex.Message}");
                return false;
            }
        }
    }
}