using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading; 
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics; 

namespace MedicalAppointmentApp.XamarinApp.Services
{
    public class AppointmentStatusDataStore : AListDataStore<AppointmentStatusForView>, IAppointmentStatusService
    {
        private readonly MedicalApiClient _apiClient;

        public AppointmentStatusDataStore()
        {
            _apiClient = DependencyService.Get<MedicalApiClient>();
            if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered.");
        }

        protected override async Task<List<AppointmentStatusForView>> GetItemsFromService()
        {
            try
            {
                ICollection<AppointmentStatusForView> result = await _apiClient.AppointmentStatusesAllAsync(CancellationToken.None);
                return result?.ToList() ?? new List<AppointmentStatusForView>();
            }
            catch (Exception ex) { Debug.WriteLine($"[AppStatusDS] GetItems Error: {ex.Message}"); return new List<AppointmentStatusForView>(); }
        }

        protected override Task<AppointmentStatusForView> GetItemFromService(int id)
        {
            try
            {
                return _apiClient.AppointmentStatusesGETAsync(id, CancellationToken.None);
            }
            catch (ApiException ex) when (ex.StatusCode == 404) { return null; }
            catch (Exception ex) { Debug.WriteLine($"[AppStatusDS] GetItem Error: {ex.Message}"); return null; }
        }

        protected override Task<AppointmentStatusForView> AddItemToService(AppointmentStatusForView item)
        {
            return _apiClient.AppointmentStatusesPOSTAsync(item, CancellationToken.None);
        }

        protected override async Task UpdateItemInService(AppointmentStatusForView item)
        {
            await _apiClient.AppointmentStatusesPUTAsync(item.StatusId, item, CancellationToken.None);
        }

        protected override async Task DeleteItemFromService(int id)
        {
            await _apiClient.AppointmentStatusesDELETEAsync(id, CancellationToken.None);
        }

        public override AppointmentStatusForView Find(int id)
        {
            return items?.FirstOrDefault(s => s.StatusId == id);
        }
    }
}