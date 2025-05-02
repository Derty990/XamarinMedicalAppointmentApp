using MedicalAppointmentApp.XamarinApp.ApiClient;
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using System.Threading.Tasks;

namespace MedicalAppointmentApp.Services.Abstract
{
    public interface IUserService : IDataStore<UserForView> 
    {
        Task<UserForView> RegisterUserAsync(UserCreateDto createDto); 
        Task UpdateUserAsync(int id, UserUpdateDto updateDto); 
    }
}