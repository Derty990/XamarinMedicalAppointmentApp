using MedicalAppointmentApp.XamarinApp.ApiClient;
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using System.Threading.Tasks;

namespace MedicalAppointmentApp.Services.Abstract
{
    public interface IAppointmentService : IDataStore<AppointmentForView> 
    {
        Task<AppointmentForView> CreateAppointmentAsync(AppointmentCreateDto createDto); 
        Task UpdateAppointmentAsync(int id, AppointmentUpdateDto updateDto);
    }
}