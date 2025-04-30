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
    public class DoctorClinicsController : ControllerBase
    {
        private readonly MedicalDbContext _context;

        public DoctorClinicsController(MedicalDbContext context)
        {
            _context = context;
        }

        // GET: api/doctorclinics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorClinic>>> GetDoctorClinics(int? doctorId, int? clinicId) // Filtry
        {
            // TODO: Użyj DTO
            var query = _context.DoctorClinics
                        .Include(dc => dc.Doctor).ThenInclude(d => d.User) // Załaduj lekarza (z User)
                        .Include(dc => dc.Clinic) // Załaduj klinikę
                        .AsQueryable();

            if (doctorId.HasValue)
            {
                query = query.Where(dc => dc.DoctorId == doctorId.Value);
            }
            if (clinicId.HasValue)
            {
                query = query.Where(dc => dc.ClinicId == clinicId.Value);
            }

            return await query.ToListAsync();
        }

        // GET: api/doctorclinics/5 (Wg ID tabeli łączącej)
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorClinic>> GetDoctorClinic(int id)
        {
            // TODO: Użyj DTO
            var doctorClinic = await _context.DoctorClinics
                                        .Include(dc => dc.Doctor).ThenInclude(d => d.User)
                                        .Include(dc => dc.Clinic)
                                        .FirstOrDefaultAsync(dc => dc.DoctorClinicId == id);


            if (doctorClinic == null)
            {
                return NotFound();
            }

            return doctorClinic;
        }

        // POST: api/doctorclinics (Dodanie powiązania lekarza z kliniką)
        [HttpPost]
        public async Task<ActionResult<DoctorClinic>> PostDoctorClinic(DoctorClinic doctorClinic) // TODO: Użyj DTO { int DoctorId; int ClinicId; }
        {
            // TODO: Walidacja - czy lekarz i klinika istnieją? Czy powiązanie już istnieje?
            bool alreadyExists = await _context.DoctorClinics.AnyAsync(dc => dc.DoctorId == doctorClinic.DoctorId && dc.ClinicId == doctorClinic.ClinicId);
            if (alreadyExists)
            {
                return BadRequest("This doctor is already assigned to this clinic.");
            }

            _context.DoctorClinics.Add(doctorClinic);
            await _context.SaveChangesAsync();

            // TODO: Zwróć DTO
            // Zwracamy akcję GetDoctorClinic, używając ID nowo utworzonego rekordu
            return CreatedAtAction(nameof(GetDoctorClinic), new { id = doctorClinic.DoctorClinicId }, doctorClinic);
        }

        // DELETE: api/doctorclinics/5 (Usunięcie powiązania wg ID tabeli łączącej)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctorClinic(int id)
        {
            var doctorClinic = await _context.DoctorClinics.FindAsync(id);
            if (doctorClinic == null)
            {
                return NotFound();
            }

            _context.DoctorClinics.Remove(doctorClinic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/doctorclinics?doctorId=1&clinicId=2 (Alternatywne usuwanie wg ID lekarza i kliniki)
        [HttpDelete]
        public async Task<IActionResult> DeleteDoctorClinicByDoctorAndClinic(int doctorId, int clinicId)
        {
            var doctorClinic = await _context.DoctorClinics
                                        .FirstOrDefaultAsync(dc => dc.DoctorId == doctorId && dc.ClinicId == clinicId);
            if (doctorClinic == null)
            {
                return NotFound("Assignment not found.");
            }

            _context.DoctorClinics.Remove(doctorClinic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT nie jest typowe dla tabeli łączącej, chyba że ma dodatkowe pola do aktualizacji.
    }
}