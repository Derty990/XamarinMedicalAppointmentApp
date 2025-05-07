using MedicalAppointmentApp.WebApi.Data;
using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.ForView;
using MedicalAppointmentApp.WebApi.Dtos;   
using MedicalAppointmentApp.WebApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using BCrypt.Net;

namespace MedicalAppointmentApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users - Zwraca listę UserForView
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserForView>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            var usersForView = users.Select(u => (UserForView)u).ToList(); // Użycie operatora
            return Ok(usersForView);
        }

        // GET: api/Users/5 - Zwraca jednego UserForView
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserForView))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserForView>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            UserForView forView = user; 
            return Ok(forView);
        }

        // POST: api/Users - Przyjmuje UserCreateDto, zwraca UserForView
        [HttpPost]
        public async Task<ActionResult<UserForView>> PostUser(UserCreateDto userCreateDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == userCreateDto.Email))
                return Conflict($"User with email '{userCreateDto.Email}' already exists.");
            if (userCreateDto.AddressId.HasValue && !await _context.Addresses.AnyAsync(a => a.AddressId == userCreateDto.AddressId.Value))
                return BadRequest($"Invalid AddressId: Address {userCreateDto.AddressId} does not exist.");
            if (string.IsNullOrEmpty(userCreateDto.Password))
                return BadRequest("Password is required.");

            // Ręczne mapowanie z UserCreateDto na Encję User
            var user = new User();
            user.CopyProperties(userCreateDto); // Kopiuje pasujące pola (FirstName, LastName, Email, RoleId, AddressId)

            // Hashowanie hasła
            try { user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreateDto.Password); }
            catch (Exception ex)
            {
                Console.WriteLine($"Error hashing password: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing password.");
            }

            _context.Users.Add(user);

          
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException creating user: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error creating user. Email might already exist or address ID is invalid.");
            }

            UserForView createdForView = user; 
            return Ok(createdForView);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserForView))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Dla brakujących danych
        public async Task<ActionResult<UserForView>> LoginUser([FromBody] UserLoginDto loginDto)
        {
           
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Email and password are required.");
            }

            // 1. Znajdź użytkownika po emailu
            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            // 2. Sprawdź, czy użytkownik istnieje i czy hasło pasuje (używając BCrypt.Verify)
            //    Sprawdzamy obie rzeczy naraz. Jeśli user jest null LUB Verify zwróci false -> błąd.
            //    Zakładamy, że user.PasswordHash nie będzie null/pusty dla istniejącego użytkownika.
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                // Dane logowania są niepoprawne (nie mówimy co dokładnie)
                return Unauthorized("Invalid credentials.");
            }

            // 3. Logowanie udane - Zwróć dane użytkownika
            UserForView userForView = user; 
            return Ok(userForView);
        }

        // PUT: api/Users/5 - Przyjmuje UserUpdateDto
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Kluczowe dla poprawnego działania klienta
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutUser(int id, UserUpdateDto userUpdateDto)
        {
            var userToUpdate = await _context.Users.FindAsync(id);
            if (userToUpdate == null) return NotFound();

            // Walidacja
            if (userToUpdate.Email != userUpdateDto.Email && await _context.Users.AnyAsync(u => u.Email == userUpdateDto.Email && u.UserId != id))
                return Conflict($"User with email '{userUpdateDto.Email}' already exists.");
            if (userUpdateDto.AddressId.HasValue && userToUpdate.AddressId != userUpdateDto.AddressId && !await _context.Addresses.AnyAsync(a => a.AddressId == userUpdateDto.AddressId.Value))
                return BadRequest($"Invalid AddressId: Address {userUpdateDto.AddressId} does not exist.");

            // Używamy CopyProperties, bo UserUpdateDto nie ma PasswordHash
            userToUpdate.CopyProperties(userUpdateDto);

            // Uproszczony try-catch
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(id)) { return NotFound(); } else { throw; }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException updating user {id}: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error updating user. Email might already exist or address ID is invalid.");
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            bool hasAppointments = await _context.Appointments.AnyAsync(a => a.PatientId == id);
            if (hasAppointments) return Conflict($"Cannot delete User ID {id} because they have associated appointments as a patient.");
            var doctorRecord = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == id);
            if (doctorRecord != null)
            {
                bool doctorHasAppointments = await _context.Appointments.AnyAsync(a => a.DoctorId == doctorRecord.DoctorId);
                if (doctorHasAppointments) return Conflict($"Cannot delete User ID {id} because they are a Doctor with associated appointments.");
            }
            _context.Users.Remove(user);
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException deleting user {id}: {ex.InnerException?.Message ?? ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting user.");
            }
            return NoContent();
        }

        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.UserId == id);
        }
    }
}