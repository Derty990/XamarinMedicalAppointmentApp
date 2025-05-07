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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AddressForView>))]
        public async Task<ActionResult<IEnumerable<AddressForView>>> GetAddresses()
        {
            var addresses = await _context.Addresses.ToListAsync();
            return Ok(addresses.Select(a => (AddressForView)a).ToList()); // Użycie operatora
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddressForView))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AddressForView>> GetAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return NotFound();
            AddressForView forView = address; // Użycie operatora
            return Ok(forView);
        }

        // POST: api/Addresses
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddressForView))] // Dodaj/Sprawdź
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Dodaj/Sprawdź
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return NotFound();

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