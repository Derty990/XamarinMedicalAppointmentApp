using MedicalAppointmentApp.XamarinApp.ApiClient;
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using System.Threading.Tasks;

namespace MedicalAppointmentApp.Services.Abstract
{
    public interface IClinicService : IDataStore<ClinicForView>
    {
        Task<ClinicForView> CreateClinicAsync(ClinicCreateDto createDto);
        Task UpdateClinicAsync(int id, ClinicCreateDto updateDto);
    }
}