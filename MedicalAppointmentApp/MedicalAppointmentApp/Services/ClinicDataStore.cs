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
    public class ClinicDataStore : AListDataStore<ClinicForView>, IClinicService
    {
        private readonly MedicalApiClient _apiClient;

        public ClinicDataStore()
        {
            _apiClient = DependencyService.Get<MedicalApiClient>();
            if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered.");
        }

        protected override async Task<List<ClinicForView>> GetItemsFromService()
        {
            try
            {
                ICollection<ClinicForView> result = await _apiClient.ClinicsAllAsync(CancellationToken.None);
                return result?.ToList() ?? new List<ClinicForView>();
            }
            catch (Exception ex) { Debug.WriteLine($"[ClinicDataStore] GetItems Error: {ex.Message}"); return new List<ClinicForView>(); }
        }

        protected override Task<ClinicForView> GetItemFromService(int id)
        {
            try
            {
                return _apiClient.ClinicsGETAsync(id, CancellationToken.None);
            }
            catch (ApiException ex) when (ex.StatusCode == 404) { return null; }
            catch (Exception ex) { Debug.WriteLine($"[ClinicDataStore] GetItem Error: {ex.Message}"); return null; }
        }
        protected override Task<ClinicForView> AddItemToService(ClinicForView item) =>
            throw new NotImplementedException("Use CreateClinicAsync with ClinicCreateDto instead.");

        protected override Task UpdateItemInService(ClinicForView item) => 
            throw new NotImplementedException("Use UpdateClinicAsync with ClinicCreateDto instead.");


        protected override async Task DeleteItemFromService(int id) 
        {
            await _apiClient.ClinicsDELETEAsync(id, CancellationToken.None);
        }
        public Task<ClinicForView> CreateClinicAsync(ClinicCreateDto createDto)
        {
            return _apiClient.ClinicsPOSTAsync(createDto, CancellationToken.None);
        }

        public Task UpdateClinicAsync(int id, ClinicCreateDto updateDto)
        {
            return _apiClient.ClinicsPUTAsync(id, updateDto, CancellationToken.None);
        }

        public override ClinicForView Find(int id)
        {
            return items?.FirstOrDefault(c => c.ClinicId == id);
        }
    }
}