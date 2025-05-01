using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using MedicalAppointmentApp.Services.Abstract;

// T w AListDataStore to GŁÓWNY typ odczytu - WYGENEROWANY UserForView
public class UserDataStore : AListDataStore<UserForView>, IUserService
{
    private readonly MedicalApiClient _apiClient; // WYGENEROWANY Klient

    public UserDataStore()
    {
        _apiClient = DependencyService.Get<MedicalApiClient>();
        if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered.");
        // Nie ładujemy 'items' w konstruktorze
    }

    // --- Implementacja metod abstrakcyjnych z AListDataStore ---

    // Pobiera listę z API
    protected override async Task<List<UserForView>> GetItemsFromService()
    {
        // Prosta obsługa błędów - zwraca null, jeśli API rzuci wyjątkiem
        // ViewModel będzie musiał obsłużyć null lub złapać wyjątek wyżej
        try
        {
            // SPRAWDŹ nazwę metody w wygenerowanym _apiClient!
            ICollection<UserForView> users = await _apiClient.UsersAllAsync();
            return users?.ToList() ?? new List<UserForView>();
        }
        catch (Exception ex) { Debug.WriteLine($"[UserDataStore] GetItemsFromService Error: {ex.Message}"); return null; }
    }

    // Pobiera jeden element z API
    protected override async Task<UserForView> GetItemFromService(int id)
    {
        try
        {
            // SPRAWDŹ nazwę metody w wygenerowanym _apiClient!
            return await _apiClient.UsersGETAsync(id);
        }
        // Można specyficznie obsłużyć 404 z ApiException, jeśli generator go rzuca
        catch (ApiException ex) when (ex.StatusCode == 404) { return null; }
        catch (Exception ex) { Debug.WriteLine($"[UserDataStore] GetItemFromService Error: {ex.Message}"); return null; }
    }

    // Usuwa element przez API
    protected override async Task<bool> DeleteItemFromService(int id)
    {
        try
        {
            // SPRAWDŹ nazwę metody w wygenerowanym _apiClient!
            // Sprawdź co zwraca - jeśli Task, to sukcesem jest brak wyjątku
            await _apiClient.UsersDELETEAsync(id);
            return true; // Sukces, jeśli doszło tutaj
        }
        catch (Exception ex) { Debug.WriteLine($"[UserDataStore] DeleteItemFromService Error: {ex.Message}"); return false; }
    }

    // Implementacja metody Find dla cache 'items'
    public override UserForView Find(int id)
    {
        // Zakładamy, że WYGENEROWANY UserForView ma właściwość UserId
        return items?.FirstOrDefault(u => u.UserId == id);
    }

    // Te metody nie pasują do rejestracji/aktualizacji Usera, bo wymagają innych DTO
    protected override Task<UserForView> AddItemToService(UserForView item) => throw new NotImplementedException("Use RegisterUserAsync instead.");
    protected override Task<bool> UpdateItemInService(UserForView item) => throw new NotImplementedException("Use UpdateUserAsync instead.");


    // --- Implementacja dedykowanych metod z IUserService ---

    // Rejestracja - używa WYGENEROWANEGO UserCreateDto i zwraca WYGENEROWANE UserForView
    public async Task<UserForView> RegisterUserAsync(UserCreateDto createDto)
    {
        try
        {
            // SPRAWDŹ nazwę metody i typy w wygenerowanym kliencie!
            return await _apiClient.UsersPOSTAsync(createDto);
        }
        catch (Exception ex) { Debug.WriteLine($"[UserDataStore] RegisterUserAsync Error: {ex.Message}"); return null; }
    }

    // Aktualizacja - używa WYGENEROWANEGO UserUpdateDto
    public async Task UpdateUserAsync(int id, UserUpdateDto updateDto) // Zmieniono zwracany typ na Task
    {
        // SPRAWDŹ nazwę metody w wygenerowanym kliencie!
        // Zakładamy, że rzuca wyjątek przy błędzie. Sukces = brak wyjątku.
        await _apiClient.UsersPUTAsync(id, updateDto);
    }
}

