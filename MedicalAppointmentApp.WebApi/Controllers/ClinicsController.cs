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
    public class ClinicsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClinicsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Clinics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicForView>>> GetClinics()
        {
            // Trzeba dołączyć Adres, aby operator konwersji zadziałał poprawnie dla spłaszczonych pól
            var clinics = await _context.Clinics
                                      .Include(c => c.Address)
                                      .ToListAsync();
            // Konwersja listy encji na listę ForView za pomocą operatora
            return Ok(clinics.Select(c => (ClinicForView)c).ToList());
        }

        // GET: api/Clinics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicForView>> GetClinic(int id)
        {
            // Trzeba dołączyć Adres
            var clinic = await _context.Clinics
                                     .Include(c => c.Address)
                                     .FirstOrDefaultAsync(c => c.ClinicId == id);

            if (clinic == null)
            {
                return NotFound();
            }
            ClinicForView forView = clinic; // Użycie operatora konwersji
            return Ok(forView);
        }

        // POST: api/Clinics
        [HttpPost]
        public async Task<ActionResult<ClinicForView>> PostClinic(ClinicCreateDto clinicCreateDto) 
        {
            // Sprawdź czy AddressId istnieje
            if (!await _context.Addresses.AnyAsync(a => a.AddressId == clinicCreateDto.AddressId))
            {
                return BadRequest($"Invalid AddressId: Address with ID {clinicCreateDto.AddressId} does not exist.");
            }

            var clinic = new Clinic();
            clinic.CopyProperties(clinicCreateDto); // Kopiuj Name, AddressId

            _context.Clinics.Add(clinic);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException creating clinic: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error creating clinic. The specified Address ID might be invalid.");
            }

            // Pobierz ponownie zapisaną klinikę Z ADRESEM, aby zwrócić pełne ForView
            var createdClinicWithAddress = await _context.Clinics
                                                       .Include(c => c.Address)
                                                       .FirstOrDefaultAsync(c => c.ClinicId == clinic.ClinicId);

            if (createdClinicWithAddress == null) // Nie powinno się zdarzyć, ale na wszelki wypadek
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not retrieve created clinic.");


            ClinicForView createdForView = createdClinicWithAddress; // Konwersja na ForView
            return CreatedAtAction(nameof(GetClinic), new { id = clinic.ClinicId }, createdForView);
        }

        // PUT: api/Clinics/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClinic(int id, ClinicCreateDto clinicUpdateDto) // Używamy CreateDto
        {
            var clinicToUpdate = await _context.Clinics.FindAsync(id);
            if (clinicToUpdate == null) return NotFound();

            // Sprawdź nowe AddressId
            if (clinicToUpdate.AddressId != clinicUpdateDto.AddressId &&
                !await _context.Addresses.AnyAsync(a => a.AddressId == clinicUpdateDto.AddressId))
            {
                return BadRequest($"Invalid AddressId: Address with ID {clinicUpdateDto.AddressId} does not exist.");
            }

            clinicToUpdate.CopyProperties(clinicUpdateDto); // Kopiuj Name i AddressId

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ClinicExists(id)) { return NotFound(); } else { throw; }
            }
            catch (DbUpdateException ex) // Uproszczona obsługa błędów FK
            {
                Console.WriteLine($"DbUpdateException updating clinic {id}: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error updating clinic. The specified Address ID might be invalid.");
            }

            return NoContent();
        }

        // DELETE: api/Clinics/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null) return NotFound();

            _context.Clinics.Remove(clinic);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                Console.WriteLine($"DbUpdateException deleting clinic {id}: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict($"Cannot delete Clinic ID {id} because it might be referenced by Doctors or Appointments.");
            }

            return NoContent();
        }

        private async Task<bool> ClinicExists(int id)
        {
            return await _context.Clinics.AnyAsync(e => e.ClinicId == id);
        }
    }
}