using MedicalAppointmentApp.XamarinApp.ApiClient;
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MedicalAppointmentApp.Services.Abstract
{
    public interface IDoctorService : IDataStore<DoctorForView> 
    {
        Task<List<DoctorListItemDto>> GetDoctorListItemsAsync();
        Task<DoctorForView> CreateDoctorAsync(DoctorCreateDto createDto); 
        Task UpdateDoctorSpecializationAsync(int id, DoctorUpdateDto updateDto); 
    }
}