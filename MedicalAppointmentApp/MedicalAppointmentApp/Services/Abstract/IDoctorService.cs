using MedicalAppointmentApp.XamarinApp.ApiClient;
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MedicalAppointmentApp.Services.Abstract
{
    public interface IDoctorService : IDataStore<DoctorForView> // T to WYGENEROWANY DoctorForView
    {
        Task<List<DoctorListItemDto>> GetDoctorListItemsAsync(); // Zwraca WYGENEROWANE DoctorListItemDto
        Task<DoctorForView> CreateDoctorAsync(DoctorCreateDto createDto); // Używa WYGENEROWANYCH DTO
        Task UpdateDoctorSpecializationAsync(int id, DoctorUpdateDto updateDto); // Zwraca Task
    }
}