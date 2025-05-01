using MedicalAppointmentApp.XamarinApp.ApiClient;
using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using System.Threading.Tasks;

namespace MedicalAppointmentApp.Services.Abstract
{
    public interface IUserService : IDataStore<UserForView> // T to WYGENEROWANE UserForView
    {
        Task<UserForView> RegisterUserAsync(UserCreateDto createDto); // Przyjmuje WYGENEROWANE UserCreateDto
        Task UpdateUserAsync(int id, UserUpdateDto updateDto); // Przyjmuje WYGENEROWANE UserUpdateDto, zwraca Task
    }
}