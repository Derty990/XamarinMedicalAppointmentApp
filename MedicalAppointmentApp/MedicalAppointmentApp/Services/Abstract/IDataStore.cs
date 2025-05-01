using System.Collections.Generic;
using System.Threading.Tasks;
namespace MedicalAppointmentApp.XamarinApp.Services.Abstract
{
    public interface IDataStore<T> where T : class
    {
        // Zwracają Task - sukces oznacza brak wyjątku
        Task AddItemAsync(T item);
        Task UpdateItemAsync(T item);
        Task DeleteItemAsync(int id);

        // Zwracają T lub null / List<T> lub pustą listę
        Task<T> GetItemAsync(int id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}