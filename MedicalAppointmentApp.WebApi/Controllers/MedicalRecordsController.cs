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
    public class MedicalRecordsController : ControllerBase
    {
        private readonly MedicalDbContext _context;

        public MedicalRecordsController(MedicalDbContext context)
        {
            _context = context;
        }

        // GET: api/medicalrecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalRecord>>> GetMedicalRecords(int? patientId) // Filtr wg pacjenta
        {
            // TODO: Użyj DTO
            var query = _context.MedicalRecords
                        .Include(mr => mr.Patient)
                        .Include(mr => mr.Appointment) // Opcjonalnie, jeśli potrzebne
                        .AsQueryable();

            if (patientId.HasValue)
            {
                query = query.Where(mr => mr.PatientId == patientId.Value);
            }

            return await query.OrderByDescending(mr => mr.RecordDate).ToListAsync();
        }

        // GET: api/medicalrecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecord>> GetMedicalRecord(int id)
        {
            // TODO: Użyj DTO
            var medicalRecord = await _context.MedicalRecords
                                        .Include(mr => mr.Patient)
                                        .Include(mr => mr.Appointment)
                                        .FirstOrDefaultAsync(mr => mr.RecordId == id);

            if (medicalRecord == null)
            {
                return NotFound();
            }

            return medicalRecord;
        }

        // PUT: api/medicalrecords/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicalRecord(int id, MedicalRecord medicalRecord) // TODO: Użyj DTO
        {
            if (id != medicalRecord.RecordId)
            {
                return BadRequest();
            }

            // TODO: Walidacja
            // TODO: Czy można modyfikować PatientId/AppointmentId? Raczej nie.

            _context.Entry(medicalRecord).State = EntityState.Modified;
            _context.Entry(medicalRecord).Property(x => x.RecordDate).IsModified = false; // Data rekordu raczej niezmienna
            _context.Entry(medicalRecord).Property(x => x.PatientId).IsModified = false;
            _context.Entry(medicalRecord).Property(x => x.AppointmentId).IsModified = false;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicalRecordExists(id))
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

        // POST: api/medicalrecords
        [HttpPost]
        public async Task<ActionResult<MedicalRecord>> PostMedicalRecord(MedicalRecord medicalRecord) // TODO: Użyj DTO
        {
            // TODO: Walidacja (czy pacjent istnieje, czy AppointmentId jest poprawne itp.)
            medicalRecord.RecordDate = DateTime.UtcNow; // Ustaw datę po stronie serwera

            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();

            // TODO: Zwróć DTO
            return CreatedAtAction(nameof(GetMedicalRecord), new { id = medicalRecord.RecordId }, medicalRecord);
        }

        // DELETE: api/medicalrecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            // Czy rekordy medyczne powinny być usuwalne? Może archiwizacja?
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord == null)
            {
                return NotFound();
            }

            _context.MedicalRecords.Remove(medicalRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicalRecordExists(int id)
        {
            return _context.MedicalRecords.Any(e => e.RecordId == id);
        }
    }
}