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

        // Abstrakcyjne metody, które klasy pochodne muszą zaimplementować
        // Te metody wykonują faktyczne wywołania API
        protected abstract Task<List<T>> GetItemsFromService(); // API powinno zwrócić listę obiektów T
        protected abstract Task<T> GetItemFromService(int id);    // API powinno zwrócić pojedynczy obiekt T

        // Dla operacji CUD, metody ...ToService mogą zwracać różne rzeczy (np. utworzony obiekt, bool, lub nic)
        // ale publiczne metody interfejsu IDataStore<T> będą zwracać Task.
        // Zmieńmy AddItemToService, aby zwracał Task, jeśli API POST nie zwraca utworzonego obiektu
        // lub jeśli nie potrzebujemy go bezpośrednio w tym miejscu. Jeśli API zwraca T, można zostawić Task<T>.
        // Dla spójności z IDataStore<T>.AddItemAsync(T item) -> Task, zrobimy Task.
        // Ale często API POST zwraca utworzony obiekt (np. z ID), więc Task<T> jest też popularne.
        // Zostawmy Task<T> dla AddItemToService, bo często się to przydaje, a publiczna metoda obsłuży Task.
        protected abstract Task<T> AddItemToService(T item);
        protected abstract Task UpdateItemInService(T item);    // PUT często zwraca 204 No Content (Task)
        protected abstract Task DeleteItemFromService(int id);  // DELETE często zwraca 204 No Content (Task)

        // Abstrakcyjna metoda Find do implementacji w klasie pochodnej (do szukania w 'items')
        public abstract T Find(int id);

        // Wirtualna metoda inicjalizująca cache
        protected virtual async Task InitializeListCacheAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && isInitialized) // Pomiń, jeśli nie wymuszono i już zainicjalizowano
            {
                return;
            }

            items = await GetItemsFromService() ?? new List<T>();
            isInitialized = true;
        }

        // Implementacja metod z interfejsu IDataStore<T>

        public async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
        {
            await InitializeListCacheAsync(forceRefresh);
            return items ?? new List<T>();
        }

        public async Task<T> GetItemAsync(int id)
        {
            // Dla uproszczenia zawsze pobieramy świeży obiekt z serwisu.
            // Można by dodać logikę sprawdzania cache 'items' najpierw, jeśli potrzebne.
            return await GetItemFromService(id);
        }

        public async Task AddItemAsync(T item)
        {
            // Wywołaj abstrakcyjną metodę serwisu.
            // Jeśli AddItemToService zwróciłoby np. utworzony obiekt z ID, można by go tu użyć.
            // Obecnie IDataStore.AddItemAsync zwraca Task, więc nie przejmujemy się wynikiem.
            await AddItemToService(item);
            isInitialized = false; // Unieważnij cache, aby następne GetItemsAsync pobrało świeżą listę
        }

        public async Task UpdateItemAsync(T item)
        {
            await UpdateItemInService(item);
            isInitialized = false; // Unieważnij cache
        }

        public async Task DeleteItemAsync(int id)
        {
            await DeleteItemFromService(id);
            isInitialized = false; // Unieważnij cache
        }
    }
}