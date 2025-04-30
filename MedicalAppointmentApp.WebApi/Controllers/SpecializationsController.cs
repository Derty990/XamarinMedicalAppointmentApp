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
    public class SpecializationsController : ControllerBase
    {
        private readonly MedicalDbContext _context;

        public SpecializationsController(MedicalDbContext context)
        {
            _context = context;
        }

        // GET: api/specializations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Specialization>>> GetSpecializations()
        {
            return await _context.Specializations.ToListAsync();
        }

        // GET: api/specializations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Specialization>> GetSpecialization(int id)
        {
            var specialization = await _context.Specializations.FindAsync(id);

            if (specialization == null)
            {
                return NotFound();
            }

            return specialization;
        }

        // PUT: api/specializations/5
        // Aby zaktualizować, wysyłasz cały obiekt Specialization w ciele żądania
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecialization(int id, Specialization specialization)
        {
            if (id != specialization.SpecializationId)
            {
                return BadRequest("ID in URL must match ID in request body.");
            }

            // TODO: Dodaj walidację dla obiektu specialization

            _context.Entry(specialization).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecializationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Rzuć wyjątek dalej, jeśli problem jest inny
                }
            }

            return NoContent(); // Sukces, brak treści w odpowiedzi
        }

        // POST: api/specializations
        // Wysyłasz obiekt Specialization (bez ID) w ciele żądania
        [HttpPost]
        public async Task<ActionResult<Specialization>> PostSpecialization(Specialization specialization)
        {
            // TODO: Dodaj walidację dla obiektu specialization
            // TODO: Rozważ użycie DTO, aby nie przyjmować SpecializationId od klienta

            _context.Specializations.Add(specialization);
            await _context.SaveChangesAsync();

            // Zwraca 201 Created z nagłówkiem Location wskazującym na nowo utworzony zasób
            return CreatedAtAction(nameof(GetSpecialization), new { id = specialization.SpecializationId }, specialization);
        }

        // DELETE: api/specializations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecialization(int id)
        {
            var specialization = await _context.Specializations.FindAsync(id);
            if (specialization == null)
            {
                return NotFound();
            }

            // TODO: Rozważ sprawdzenie, czy jacyś lekarze nie są powiązani z tą specjalizacją
            // i ewentualnie zabroń usunięcia lub ustaw ich SpecializationId na null.

            _context.Specializations.Remove(specialization);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpecializationExists(int id)
        {
            return _context.Specializations.Any(e => e.SpecializationId == id);
        }
    }
}