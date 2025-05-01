using MedicalAppointmentApp.XamarinApp.ApiClient;
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using System.Threading.Tasks;

namespace MedicalAppointmentApp.Services.Abstract
{
    public interface IAddressService : IDataStore<AddressForView>
    {
        Task<AddressForView> CreateAddressAsync(AddressCreateDto createDto);
        Task UpdateAddressAsync(int id, AddressCreateDto updateDto); // Zwraca Task
    }
}