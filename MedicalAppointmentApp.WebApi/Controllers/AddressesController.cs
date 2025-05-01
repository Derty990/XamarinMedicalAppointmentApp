using MedicalAppointmentApp.WebApi.Data;
using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.ForView; // Używamy ForView
using MedicalAppointmentApp.WebApi.Dtos;    // Używamy CreateDto
using MedicalAppointmentApp.WebApi.Helpers; // Używamy PropertyUtil
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MedicalAppointmentApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AddressesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressForView>>> GetAddresses()
        {
            var addresses = await _context.Addresses.ToListAsync();
            return Ok(addresses.Select(a => (AddressForView)a).ToList()); // Użycie operatora
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressForView>> GetAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return NotFound();
            AddressForView forView = address; // Użycie operatora
            return Ok(forView);
        }

        // POST: api/Addresses
        [HttpPost]
        public async Task<ActionResult<AddressForView>> PostAddress(AddressCreateDto addressCreateDto) // Przyjmuje CreateDto
        {
            var address = new Address(); // Tworzymy nową encję
            address.CopyProperties(addressCreateDto); // Kopiujemy pola z DTO

            _context.Addresses.Add(address);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) // Uproszczona obsługa
            {
                Console.WriteLine($"DbUpdateException creating address: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error creating address.");
            }

            AddressForView createdForView = address; // Konwersja do ForView
            return CreatedAtAction(nameof(GetAddress), new { id = address.AddressId }, createdForView);
        }

        // PUT: api/Addresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, AddressCreateDto addressUpdateDto) // Przyjmuje CreateDto
        {
            var addressToUpdate = await _context.Addresses.FindAsync(id);
            if (addressToUpdate == null) return NotFound();

            addressToUpdate.CopyProperties(addressUpdateDto); // Aktualizacja pól

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AddressExists(id)) { return NotFound(); } else { throw; }
            }
            catch (DbUpdateException ex) // Uproszczona obsługa
            {
                Console.WriteLine($"DbUpdateException updating address {id}: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error updating address.");
            }

            return NoContent();
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return NotFound();

            // Sprawdzenie czy adres jest używany (uproszczone, polegamy na błędzie FK z bazy)
            // Można dodać jawne sprawdzenie jak wcześniej, jeśli chcemy lepszy komunikat

            _context.Addresses.Remove(address);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) // Uproszczona obsługa błędów FK
            {
                Console.WriteLine($"DbUpdateException deleting address {id}: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict($"Cannot delete Address ID {id} because it might be in use by a Clinic or User.");
            }

            return NoContent();
        }

        private async Task<bool> AddressExists(int id)
        {
            return await _context.Addresses.AnyAsync(e => e.AddressId == id);
        }
    }
}