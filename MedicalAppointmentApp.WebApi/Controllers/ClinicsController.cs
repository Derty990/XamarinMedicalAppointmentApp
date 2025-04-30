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
    public class ClinicsController : ControllerBase
    {
        private readonly MedicalDbContext _context;

        public ClinicsController(MedicalDbContext context)
        {
            _context = context;
        }

        // GET: api/clinics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clinic>>> GetClinics(bool onlyActive = true) // Opcjonalny parametr
        {
            if (onlyActive)
            {
                return await _context.Clinics.Where(c => c.IsActive).ToListAsync();
            }
            return await _context.Clinics.ToListAsync();
        }

        // GET: api/clinics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Clinic>> GetClinic(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            // Można też załadować powiązane dane, jeśli są potrzebne od razu
            // var clinic = await _context.Clinics.Include(c => c.DoctorClinics).FirstOrDefaultAsync(c => c.ClinicId == id);

            if (clinic == null)
            {
                return NotFound();
            }

            return clinic;
        }

        // PUT: api/clinics/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClinic(int id, Clinic clinic)
        {
            if (id != clinic.ClinicId)
            {
                return BadRequest("ID in URL must match ID in request body.");
            }

            // TODO: Użyj DTO
            // TODO: Dodaj walidację

            _context.Entry(clinic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClinicExists(id))
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

        // POST: api/clinics
        [HttpPost]
        public async Task<ActionResult<Clinic>> PostClinic(Clinic clinic)
        {
            // TODO: Użyj DTO
            // TODO: Dodaj walidację

            _context.Clinics.Add(clinic);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClinic), new { id = clinic.ClinicId }, clinic);
        }

        // DELETE: api/clinics/5
        // Zamiast fizycznego usuwania, często lepszą praktyką jest "miękkie usuwanie" (Soft Delete)
        // poprzez ustawienie flagi IsActive = false. Poniżej przykład miękkiego usuwania.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
            {
                return NotFound();
            }

            if (!clinic.IsActive) // Jeśli już jest nieaktywna, nic nie rób
            {
                return NoContent();
            }

            // Sprawdź czy są aktywne wizyty w tej klinice?
            bool hasActiveAppointments = await _context.Appointments
                .AnyAsync(a => a.ClinicId == id && a.AppointmentDate >= DateTime.UtcNow.Date && a.Status == "Scheduled");

            if (hasActiveAppointments)
            {
                return BadRequest("Cannot deactivate clinic with upcoming scheduled appointments.");
            }

            // Miękkie usuwanie
            clinic.IsActive = false;
            _context.Entry(clinic).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();

            /* // Kod dla fizycznego usuwania (jeśli preferowane):
            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();
            return NoContent();
            */
        }

        private bool ClinicExists(int id)
        {
            return _context.Clinics.Any(e => e.ClinicId == id);
        }
    }
}