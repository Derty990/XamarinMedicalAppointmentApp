using MedicalAppointmentApp.XamarinApp.Services.Abstract;
using MedicalAppointmentApp.XamarinApp.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using MedicalAppointmentApp.Services.Abstract;

public class UserDataStore : AListDataStore<UserForView>, IUserService
{
    private readonly MedicalApiClient _apiClient; 

    public UserDataStore()
    {
        _apiClient = DependencyService.Get<MedicalApiClient>();
        if (_apiClient == null) throw new InvalidOperationException($"{nameof(MedicalApiClient)} not registered.");
       
    }


    protected override async Task<List<UserForView>> GetItemsFromService()
    {
       
        try
        {
           
            ICollection<UserForView> users = await _apiClient.UsersAllAsync();
            return users?.ToList() ?? new List<UserForView>();
        }
        catch (Exception ex) { Debug.WriteLine($"[UserDataStore] GetItemsFromService Error: {ex.Message}"); return null; }
    }

   
    protected override async Task<UserForView> GetItemFromService(int id)
    {
        try
        {
           
            return await _apiClient.UsersGETAsync(id);
        }
        
        catch (ApiException ex) when (ex.StatusCode == 404) { return null; }
        catch (Exception ex) { Debug.WriteLine($"[UserDataStore] GetItemFromService Error: {ex.Message}"); return null; }
    }

   
    protected override async Task DeleteItemFromService(int id)
    {
        try
        {
          
            await _apiClient.UsersDELETEAsync(id);
            
        }
        catch (Exception ex) { Debug.WriteLine($"[UserDataStore] DeleteItemFromService Error: {ex.Message}"); }
    }

   
    public override UserForView Find(int id)
    {
       
        return items?.FirstOrDefault(u => u.UserId == id);
    }

   
    protected override Task<UserForView> AddItemToService(UserForView item) => throw new NotImplementedException("Use RegisterUserAsync instead.");
    protected override Task UpdateItemInService(UserForView item) => throw new NotImplementedException("Use UpdateUserAsync instead.");


  
    public async Task<UserForView> RegisterUserAsync(UserCreateDto createDto)
    {
        try
        {
            
            return await _apiClient.UsersPOSTAsync(createDto);
        }
        catch (Exception ex) { Debug.WriteLine($"[UserDataStore] RegisterUserAsync Error: {ex.Message}"); return null; }
    }

   
    public async Task UpdateUserAsync(int id, UserUpdateDto updateDto)
    {
        
        await _apiClient.UsersPUTAsync(id, updateDto);
    }

    public async Task<UserForView> LoginUserAsync(string email, string password)
    {
        var loginDto = new UserLoginDto { Email = email, Password = password };
        try
        {
           
            UserForView loggedInUser = await _apiClient.LoginAsync(loginDto); 
                                                                              
            return loggedInUser; 
        }
        catch (ApiException ex) when (ex.StatusCode == 401 || ex.StatusCode == 404 || ex.StatusCode == 400)
        {
            Debug.WriteLine($"[UserDataStore] Login Failed: Status {ex.StatusCode}, Response: {ex.Response}");
            return null; // Błąd logowania
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UserDataStore] LoginUserAsync Unexpected Error: {ex.Message}");
            
            throw new Exception("Exception in LoginUserAsync");
        }
    }
}

