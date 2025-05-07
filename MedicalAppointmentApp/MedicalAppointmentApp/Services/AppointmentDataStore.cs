using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using MedicalAppointmentApp.Services.Abstract;

namespace MedicalAppointmentApp.XamarinApp.Services
{
    public class AppointmentDataStore : AListDataStore<AppointmentForView>, IAppointmentService
    {
        private readonly MedicalApiClient _apiClient;

        public AppointmentDataStore()
        {
            _apiClient = DependencyService.Get<MedicalApiClient>();
            if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered.");
        }
        protected override async Task<List<AppointmentForView>> GetItemsFromService()
        {
            try {
               
                ICollection<AppointmentForView> result = await _apiClient.AppointmentsAllAsync();
                return result?.ToList() ?? new List<AppointmentForView>();
            }
            catch (Exception ex) { Debug.WriteLine($"[AppointmentDataStore] GetItemsFromService Error: {ex.Message}"); return null; }
        }

        protected override async Task<AppointmentForView> GetItemFromService(int id)
        {
            try {
                
                return await _apiClient.AppointmentsGETAsync(id);
            }
            catch (ApiException ex) when (ex.StatusCode == 404) { return null; }
            catch (Exception ex) { Debug.WriteLine($"[AppointmentDataStore] GetItemFromService Error: {ex.Message}"); return null; }
        }
       
        protected override Task DeleteItemFromService(int id)
        {
          
            return CallApiAndReturnBool(async () => await _apiClient.AppointmentsDELETEAsync(id));
        }
      
        public override AppointmentForView Find(int id)
        {
            
            return items?.FirstOrDefault(a => a.AppointmentId == id);
        }
        protected override Task<AppointmentForView> AddItemToService(AppointmentForView item) => throw new NotImplementedException("Use CreateAppointmentAsync instead.");
        protected override Task UpdateItemInService(AppointmentForView item) => throw new NotImplementedException("Use UpdateAppointmentAsync instead.");
        public Task<AppointmentForView> CreateAppointmentAsync(AppointmentCreateDto createDto)
        {
           
            return _apiClient.AppointmentsPOSTAsync(createDto);
          
        }
        public Task UpdateAppointmentAsync(int id, AppointmentUpdateDto updateDto)
        {
            
            return _apiClient.AppointmentsPUTAsync(id, updateDto);
           
        }
        private async Task<bool> CallApiAndReturnBool(Func<Task> apiCall)
        {
            try { await apiCall(); return true; }
            catch (ApiException ex) when (ex.StatusCode == 404) { return false; }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"API call failed: {ex.Message}"); return false; }
        }
    }

}
