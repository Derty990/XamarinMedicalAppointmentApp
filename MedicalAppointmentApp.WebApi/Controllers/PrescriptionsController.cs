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
    public class PrescriptionsController : ControllerBase
    {
        private readonly MedicalDbContext _context;

        public PrescriptionsController(MedicalDbContext context)
        {
            _context = context;
        }

        // GET: api/prescriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptions(int? patientId, int? doctorId) // Filtry
        {
            // TODO: Użyj DTO
            var query = _context.Prescriptions
                        .Include(p => p.Patient)
                        .Include(p => p.Doctor).ThenInclude(d => d.User)
                        .Include(p => p.Appointment) // Opcjonalnie
                        .AsQueryable();

            if (patientId.HasValue)
            {
                query = query.Where(p => p.PatientId == patientId.Value);
            }
            if (doctorId.HasValue)
            {
                query = query.Where(p => p.DoctorId == doctorId.Value);
            }

            return await query.OrderByDescending(p => p.IssuedDate).ToListAsync();
        }

        // GET: api/prescriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Prescription>> GetPrescription(int id)
        {
            // TODO: Użyj DTO
            var prescription = await _context.Prescriptions
                                        .Include(p => p.Patient)
                                        .Include(p => p.Doctor).ThenInclude(d => d.User)
                                        .Include(p => p.Appointment)
                                        .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return prescription;
        }

        // PUT: api/prescriptions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrescription(int id, Prescription prescription) // TODO: Użyj DTO
        {
            // Czy recepty można modyfikować? Zwykle nie, wystawia się nową.
            // Może tylko anulowanie? Albo dodanie notatki?
            // Ten endpoint może nie być potrzebny lub mieć inne znaczenie.
            if (id != prescription.PrescriptionId)
            {
                return BadRequest();
            }

            // TODO: Walidacja
            _context.Entry(prescription).State = EntityState.Modified;
            // Zablokuj modyfikację kluczowych pól
            _context.Entry(prescription).Property(x => x.IssuedDate).IsModified = false;
            _context.Entry(prescription).Property(x => x.PatientId).IsModified = false;
            _context.Entry(prescription).Property(x => x.DoctorId).IsModified = false;
            _context.Entry(prescription).Property(x => x.AppointmentId).IsModified = false;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrescriptionExists(id))
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

        // POST: api/prescriptions
        [HttpPost]
        public async Task<ActionResult<Prescription>> PostPrescription(Prescription prescription) // TODO: Użyj DTO
        {
            // TODO: Walidacja (czy pacjent, lekarz istnieją, czy lek jest poprawny itp.)
            prescription.IssuedDate = DateTime.UtcNow; // Data wystawienia po stronie serwera

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            // TODO: Zwróć DTO
            return CreatedAtAction(nameof(GetPrescription), new { id = prescription.PrescriptionId }, prescription);
        }

        // DELETE: api/prescriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            // Czy recepty można usuwać? Zwykle nie. Może oznaczenie jako anulowana?
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }

            // TODO: Logika biznesowa - czy można usunąć/anulować?

            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PrescriptionExists(int id)
        {
            return _context.Prescriptions.Any(e => e.PrescriptionId == id);
        }
    }
}