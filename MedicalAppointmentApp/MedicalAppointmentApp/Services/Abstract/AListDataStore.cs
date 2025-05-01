using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace MedicalAppointmentApp.XamarinApp.Services.Abstract
{
    public abstract class AListDataStore<T> : IDataStore<T> where T : class
    {
        protected List<T> items; // Prosty cache
        protected bool isInitialized = false; // Czy cache został zainicjowany?

        // Abstrakcyjne metody - komunikacja z API - implementowane w klasach dziedziczących
        // Zwracają teraz bardziej użyteczne typy
        protected abstract Task<T> AddItemToService(T item); // Zwraca dodany item (z ID) lub null
        protected abstract Task<bool> UpdateItemInService(T item); // Zwraca true/false
        protected abstract Task<bool> DeleteItemFromService(int id); // Zwraca true/false
        protected abstract Task<List<T>> GetItemsFromService(); // Zwraca listę lub null/pustą listę
        protected abstract Task<T> GetItemFromService(int id); // Zwraca item lub null

        // Abstrakcyjne metody Find - operują na cache 'items'
        public abstract T Find(int id);
        // Można dodać więcej metod Find, jeśli potrzebne

        // Inicjalizacja cache'u (może być wywołana z konstruktora klasy dziedziczącej lub leniwie)
        protected virtual async Task InitializeAsync()
        {
            if (isInitialized)
                return;

            items = await GetItemsFromService() ?? new List<T>(); // Pobierz z API
            isInitialized = true;
        }

        // Publiczne metody interfejsu - operują na cache i wywołują metody ...ToService
        // BEZ BLOKÓW TRY-CATCH - wyjątki polecą do ViewModelu

        public async Task<bool> AddItemAsync(T item)
        {
            T addedItem = await AddItemToService(item); // Wywołanie metody z API
            if (addedItem != null)
            {
                await InitializeAsync(); // Upewnij się, że cache jest zainicjowany
                items?.Add(addedItem); // Dodaj do cache
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateItemAsync(T item)
        {
            bool success = await UpdateItemInService(item); // Wywołanie metody z API
            if (success)
            {
                // Odświeżamy cały cache - proste, ale mniej wydajne
                // Lepsze byłoby znalezienie i podmiana elementu w `items`
                isInitialized = false; // Oznacz cache jako nieaktualny
                await GetItemsAsync(true); // Wymuś odświeżenie przy następnym pobraniu listy
            }
            return success;
        }

        public virtual async Task<bool> DeleteItemAsync(int id)
        {
            // Najpierw znajdź w cache, żeby mieć pewność, że istniał (opcjonalne)
            // T itemToDelete = Find(id);
            // if (itemToDelete == null) return false; // Lub rzuć wyjątek?

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
            await InitializeAsync(); // Upewnij się, że cache jest załadowany
            T item = Find(id); // Szukaj w cache
            if (item == null)
            {
                // Jeśli nie ma w cache, spróbuj pobrać z API
                item = await GetItemFromService(id);
                // Można dodać do cache, jeśli znaleziono i cache istnieje
                // if (item != null && items != null) items.Add(item);
            }
            return item; // Może zwrócić null, jeśli nie ma ani w cache, ani w API
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