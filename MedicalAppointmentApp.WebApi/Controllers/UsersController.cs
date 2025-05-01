using MedicalAppointmentApp.WebApi.Data;
using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.ForView; // Do odpowiedzi
using MedicalAppointmentApp.WebApi.Dtos;    // Do przyjmowania danych (Create/Update)
using MedicalAppointmentApp.WebApi.Helpers; // Dla CopyProperties
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using BCrypt.Net; // Using dla BCrypt

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
        public async Task<ActionResult<UserForView>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            UserForView forView = user; // Użycie operatora
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

            // Uproszczony try-catch
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException creating user: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error creating user. Email might already exist or address ID is invalid.");
            }

            UserForView createdForView = user; // Konwersja zapisanej encji na ForView
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, createdForView);
        }

        // PUT: api/Users/5 - Przyjmuje UserUpdateDto
        [HttpPut("{id}")]
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

        // DELETE: api/Users/5 - Bez zmian
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