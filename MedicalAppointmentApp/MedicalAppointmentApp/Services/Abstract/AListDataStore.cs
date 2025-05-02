using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace MedicalAppointmentApp.XamarinApp.Services.Abstract
{
    public abstract class AListDataStore<T> : IDataStore<T> where T : class
    {
        protected List<T> items; 
        protected bool isInitialized = false;

        // Abstrakcyjne metody - komunikacja z API - implementowane w klasach dziedziczących
        // Zwracają teraz bardziej użyteczne typy
        protected abstract Task<T> AddItemToService(T item); // Zwraca dodany item (z ID) lub null
        protected abstract Task<bool> UpdateItemInService(T item); // Zwraca true/false
        protected abstract Task<bool> DeleteItemFromService(int id); // Zwraca true/false
        protected abstract Task<List<T>> GetItemsFromService(); // Zwraca listę lub null/pustą listę
        protected abstract Task<T> GetItemFromService(int id); // Zwraca item lub null

       
        public abstract T Find(int id);
        // Można dodać więcej metod Find, jeśli potrzebne

       
        protected virtual async Task InitializeAsync()
        {
            if (isInitialized)
                return;

            items = await GetItemsFromService() ?? new List<T>(); // Pobierz z API
            isInitialized = true;
        }

       
        // BEZ BLOKÓW TRY-CATCH - wyjątki polecą do ViewModelu

        public async Task<bool> AddItemAsync(T item)
        {
            T addedItem = await AddItemToService(item); // Wywołanie metody z API
            if (addedItem != null)
            {
                await InitializeAsync();
                items?.Add(addedItem);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateItemAsync(T item)
        {
            bool success = await UpdateItemInService(item);
            if (success)
            {
              
                isInitialized = false; 
                await GetItemsAsync(true); // Wymuś odświeżenie przy następnym pobraniu listy
            }
            return success;
        }

        public virtual async Task<bool> DeleteItemAsync(int id)
        {
          
           

            bool success = await DeleteItemFromService(id); // Wywołanie metody z API
            if (success)
            {
                // Odświeżamy cache
                isInitialized = false;
                await GetItemsAsync(true);
            }
            return success;
        }

        public async Task<T> GetItemAsync(int id)
        {
            await InitializeAsync();
            T item = Find(id); 
            if (item == null)
            {
               
                item = await GetItemFromService(id);
               
            }
            return item; 
        }

        public async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh || !isInitialized)
            {
                await InitializeAsync(); // Wywołuje GetItemsFromService wewnątrz
            }
            return items ?? new List<T>();
        }

        Task IDataStore<T>.AddItemAsync(T item)
        {
            return AddItemAsync(item);
        }

        Task IDataStore<T>.UpdateItemAsync(T item)
        {
            return UpdateItemAsync(item);
        }

        Task IDataStore<T>.DeleteItemAsync(int id)
        {
            return DeleteItemAsync(id);
        }
    }
}