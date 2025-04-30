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
    public class DoctorsController : ControllerBase
    {
        private readonly MedicalDbContext _context;

        public DoctorsController(MedicalDbContext context)
        {
            _context = context;
        }

        // GET: api/doctors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            // TODO: Użyj DTO! Zwracanie User z PasswordHash jest niebezpieczne.
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .ToListAsync();
        }

        // GET: api/doctors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            // TODO: Użyj DTO!
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                // Można też załadować kliniki, w których pracuje
                .Include(d => d.DoctorClinics).ThenInclude(dc => dc.Clinic)
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (doctor == null)
            {
                return NotFound();
            }

            return doctor;
        }

        // PUT: api/doctors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, Doctor doctor)
        {
            // TODO: Użyj DTO! Aktualizacja profilu lekarza powinna być bardziej kontrolowana.
            // TODO: Walidacja
            if (id != doctor.DoctorId)
            {
                return BadRequest();
            }

            // Unikaj aktualizacji UserId, jeśli nie jest to zamierzone
            _context.Entry(doctor).State = EntityState.Modified;
            _context.Entry(doctor).Property(x => x.UserId).IsModified = false; // Przykład unikania modyfikacji UserId

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
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

        // POST: api/doctors
        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
        {
            // TODO: Użyj DTO! Przyjmowanie pełnej encji jest ryzykowne.
            // TODO: Walidacja (np. czy User o podanym UserId istnieje i ma rolę lekarza?)
            // TODO: Upewnij się, że UserId jest unikalne (dodaj sprawdzenie lub polegaj na ograniczeniu UNIQUE w bazie)

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            // TODO: Zwróć DTO!
            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.DoctorId }, doctor);
        }

        // DELETE: api/doctors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            // TODO: Co z powiązanymi wizytami, receptami? Kaskadowe usuwanie? Anulowanie wizyt?
            // Relacja z Users jest CASCADE, więc usunięcie User usunie Doctor.
            // Usunięcie Doctor tutaj może być problematyczne jeśli User nadal istnieje.
            // Rozważ miękkie usuwanie (dezaktywacja Usera?) lub usuwanie całego Usera.

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.DoctorId == id);
        }
    }
}