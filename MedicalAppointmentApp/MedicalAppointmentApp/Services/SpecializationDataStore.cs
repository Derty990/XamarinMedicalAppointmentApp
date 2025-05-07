// W Xamarin - Services/SpecializationDataStore.cs
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MedicalAppointmentApp.XamarinApp.Services 
{
  
    public class SpecializationDataStore : AListDataStore<SpecializationForView>, IDataStore<SpecializationForView>, ISpecializationService 
    {
        private readonly MedicalApiClient _apiClient; // WYGENEROWANY Klient

        public SpecializationDataStore()
        {
            _apiClient = DependencyService.Get<MedicalApiClient>();
            if (_apiClient == null)
            {
                throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered with DependencyService.");
            }
        }

        protected override async Task<List<SpecializationForView>> GetItemsFromService()
        {
         
            ICollection<SpecializationForView> specializations = await _apiClient.SpecializationsAllAsync();
            return specializations?.ToList() ?? new List<SpecializationForView>();
          
        }

        protected override Task<SpecializationForView> GetItemFromService(int id)
        {
           
            return _apiClient.SpecializationsGETAsync(id);
           
        }
 
        protected override Task<SpecializationForView> AddItemToService(SpecializationForView item)
        {
            return _apiClient.SpecializationsPOSTAsync(item);
          
        }

        protected override async Task UpdateItemInService(SpecializationForView item) 
        {
            
            await _apiClient.SpecializationsPUTAsync(item.SpecializationId, item, System.Threading.CancellationToken.None); 
        }


        protected override async Task DeleteItemFromService(int id) 
        {
            await _apiClient.SpecializationsDELETEAsync(id, System.Threading.CancellationToken.None);
        }

        public override SpecializationForView Find(int id)
        {
          
            return items?.FirstOrDefault(s => s.SpecializationId == id);
        }

        private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall)
        {
            await apiCall(); 
            return true; 
                        
        }
    }
}