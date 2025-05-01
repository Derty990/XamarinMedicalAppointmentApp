using MedicalAppointmentApp.XamarinApp.ApiClient;
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using System.Threading.Tasks;

namespace MedicalAppointmentApp.Services.Abstract
{
    public interface IAppointmentService : IDataStore<AppointmentForView> // T to WYGENEROWANY AppointmentForView
    {
        Task<AppointmentForView> CreateAppointmentAsync(AppointmentCreateDto createDto); // Używa WYGENEROWANYCH DTO
        Task UpdateAppointmentAsync(int id, AppointmentUpdateDto updateDto); // Zwraca Task
    }
}