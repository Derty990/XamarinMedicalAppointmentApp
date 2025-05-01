using MedicalAppointmentApp.WebApi.Data;
using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.ForView; // Używamy ForView
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using MedicalAppointmentApp.WebApi.Helpers; // Dla Console.WriteLine

namespace MedicalAppointmentApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        // Usunięto IMapper

        public SpecializationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Specializations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecializationForView>>> GetSpecializations()
        {
            var specializations = await _context.Specializations.ToListAsync();
            // Używamy operatora konwersji (zakładamy, że jest zdefiniowany w SpecializationForView)
            var specializationsForView = specializations.Select(s => (SpecializationForView)s).ToList();
            return Ok(specializationsForView);
        }

        // GET: api/Specializations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecializationForView>> GetSpecialization(int id)
        {
            var specialization = await _context.Specializations.FindAsync(id);

            if (specialization == null)
            {
                return NotFound();
            }

            SpecializationForView specializationForView = specialization; // Użycie operatora konwersji
            return Ok(specializationForView);
        }

        // POST: api/Specializations
        [HttpPost]
        public async Task<ActionResult<SpecializationForView>> PostSpecialization(SpecializationForView specializationForView)
        {
            // Używamy operatora konwersji ForView -> Encja (jeśli zdefiniowany)
            // lub mapujemy ręcznie/CopyProperties
            Specialization specialization = specializationForView; // Zakłada operator w SpecializationForView lub Encji
            if (specialization == null)
            {
                // Jeśli konwersja zawiedzie lub DTO jest puste
                return BadRequest("Invalid input data.");
            }
            // Wyzeruj ID, aby baza nadała nowe (jeśli klucz to Identity)
            specialization.SpecializationId = 0;

            _context.Specializations.Add(specialization);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) // Uproszczona obsługa
            {
                Console.WriteLine($"DbUpdateException creating specialization: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error creating specialization. Name might already exist.");
            }

            SpecializationForView createdForView = specialization; // Konwersja zapisanej encji do ForView
            return CreatedAtAction(nameof(GetSpecialization), new { id = specialization.SpecializationId }, createdForView);
        }

        // PUT: api/Specializations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecialization(int id, SpecializationForView specializationForView)
        {
            // Znajdź istniejącą encję
            var specializationToUpdate = await _context.Specializations.FindAsync(id);
            if (specializationToUpdate == null)
            {
                return NotFound();
            }

            // Użyj CopyProperties z helpera do zaktualizowania pól (bezpieczniejsze niż operator)
            specializationToUpdate.CopyProperties(specializationForView); // Kopiuje tylko pasujące właściwości

            // _context.Entry(specializationToUpdate).State = EntityState.Modified; // EF Core powinien sam wykryć zmiany

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SpecializationExists(id)) { return NotFound(); } else { throw; }
            }
            catch (DbUpdateException ex) // Uproszczona obsługa
            {
                Console.WriteLine($"DbUpdateException updating specialization {id}: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error updating specialization. Name might already exist.");
            }

            return NoContent();
        }

        // DELETE: api/Specializations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecialization(int id)
        {
            var specialization = await _context.Specializations.FindAsync(id);
            if (specialization == null)
            {
                return NotFound();
            }

            _context.Specializations.Remove(specialization);
            await _context.SaveChangesAsync(); // Uproszczona obsługa błędów (ewentualny błąd da 500)

            return NoContent();
        }

        private async Task<bool> SpecializationExists(int id)
        {
            return await _context.Specializations.AnyAsync(e => e.SpecializationId == id);
        }
    }
}