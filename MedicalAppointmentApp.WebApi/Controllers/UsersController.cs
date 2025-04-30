using MedicalAppointmentApp.WebApi.Data;
using MedicalAppointmentApp.WebApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAppointmentApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MedicalDbContext _context;

        public UsersController(MedicalDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(bool onlyActive = true)
        {
            // !!! UWAGA: NIGDY nie zwracaj haseł (nawet hashy) w rzeczywistej aplikacji !!!
            // TODO: Użyj DTO (Data Transfer Object), aby wybrać tylko potrzebne pola bez PasswordHash!
            if (onlyActive)
            {
                return await _context.Users.Where(u => u.IsActive).ToListAsync();
            }
            return await _context.Users.ToListAsync();
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            // !!! UWAGA: NIGDY nie zwracaj haseł (nawet hashy) w rzeczywistej aplikacji !!!
            // TODO: Użyj DTO!
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            // !!! UWAGA: NIGDY nie aktualizuj PasswordHash w ten sposób !!!
            // TODO: Użyj DTO! Oddzielna metoda do zmiany hasła.
            // TODO: Dodaj walidację
            if (id != user.UserId)
            {
                return BadRequest();
            }

            // Unikaj nadpisywania hasła i daty rejestracji przy aktualizacji innych danych
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return NotFound();

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.Gender = user.Gender;
            existingUser.Phone = user.Phone;
            existingUser.Address = user.Address;
            existingUser.RoleId = user.RoleId; // Czy rola może być zmieniana?
            existingUser.IsActive = user.IsActive;
            // existingUser.PasswordHash = user.PasswordHash; // NIE!!!
            // existingUser.RegisteredDate = user.RegisteredDate; // Raczej nie

            _context.Entry(existingUser).State = EntityState.Modified;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/users (Rejestracja)
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // !!! UWAGA: Tutaj powinno być hashowanie hasła przed zapisem !!!
            // TODO: Użyj DTO (np. RegisterUserDto z polem Password, a nie PasswordHash)
            // TODO: Dodaj walidację (np. czy email już istnieje)
            // TODO: Implementacja hashowania hasła (np. BCrypt.Net)
            // user.PasswordHash = HashPassword(userDto.Password); // Przykładowo

            // Ustaw datę rejestracji po stronie serwera
            user.RegisteredDate = DateTime.UtcNow;
            user.IsActive = true; // Zazwyczaj nowy użytkownik jest aktywny

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // TODO: Zwróć DTO bez PasswordHash!
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        // DELETE: api/users/5 (Miękkie usuwanie)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!user.IsActive) return NoContent();

            // TODO: Co z powiązanymi danymi? Wizytami? Lekarzem?

            user.IsActive = false; // Miękkie usuwanie
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}