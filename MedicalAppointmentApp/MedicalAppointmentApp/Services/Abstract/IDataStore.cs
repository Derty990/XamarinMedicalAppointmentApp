using System.Collections.Generic;
using System.Threading.Tasks;
namespace MedicalAppointmentApp.XamarinApp.Services.Abstract
{
    public interface IDataStore<T> where T : class
    {
       
        Task AddItemAsync(T item);
        Task UpdateItemAsync(T item);
        Task DeleteItemAsync(int id);

       
        Task<T> GetItemAsync(int id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}