using MedicalAppointmentApp.WebApi.Data;
using MedicalAppointmentApp.WebApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAppointmentApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly MedicalDbContext _context;

        public AppointmentsController(MedicalDbContext context)
        {
            _context = context;
        }

        // GET: api/appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments(int? patientId, int? doctorId, string? status) // Przykładowe filtry
        {
            // TODO: Użyj DTO!
            var query = _context.Appointments
                        .Include(a => a.Patient)
                        .Include(a => a.Doctor).ThenInclude(d => d.User)
                        .Include(a => a.Clinic)
                        .AsQueryable(); // Umożliwia dodawanie warunków

            if (patientId.HasValue)
            {
                query = query.Where(a => a.PatientId == patientId.Value);
            }
            if (doctorId.HasValue)
            {
                query = query.Where(a => a.DoctorId == doctorId.Value);
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            return await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            // TODO: Użyj DTO!
            var appointment = await _context.Appointments
                                            .Include(a => a.Patient)
                                            .Include(a => a.Doctor).ThenInclude(d => d.User)
                                            .Include(a => a.Clinic)
                                            .Include(a => a.Prescriptions) // Załaduj powiązane recepty
                                            .Include(a => a.MedicalRecord) // Załaduj powiązany rekord medyczny
                                            .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }

        // PUT: api/Appointments/5 (np. zmiana statusu, notatek)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment) // TODO: Użyj DTO dla aktualizacji!
        {
            if (id != appointment.AppointmentId)
            {
                return BadRequest();
            }

            // TODO: Walidacja! Czy można zmienić status? Czy data jest poprawna?
            // TODO: Zamiast przyjmować całą encję, lepiej przyjmować DTO np. UpdateAppointmentStatusDto { string Status; string Notes; }
            // i aktualizować tylko te pola w encji pobranej z bazy.

            _context.Entry(appointment).State = EntityState.Modified;
            // Unikaj modyfikacji pól, których nie powinno się zmieniać przez PUT, np. daty utworzenia
            _context.Entry(appointment).Property(x => x.CreatedDate).IsModified = false;
            _context.Entry(appointment).Property(x => x.PatientId).IsModified = false; // Zazwyczaj nie zmienia się pacjenta/lekarza/kliniki w ten sposób
            _context.Entry(appointment).Property(x => x.DoctorId).IsModified = false;
            _context.Entry(appointment).Property(x => x.ClinicId).IsModified = false;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
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

        // POST: api/Appointments (Rezerwacja wizyty)
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment) // TODO: Użyj DTO np. BookAppointmentDto!
        {
            // TODO: Walidacja! Czy termin jest dostępny? Czy pacjent/lekarz/klinika istnieją? Czy pacjent to nie lekarz? Itp.
            // TODO: Ustaw CreatedDate po stronie serwera
            appointment.CreatedDate = DateTime.UtcNow;
            appointment.Status = "Scheduled"; // Ustaw domyślny status

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // TODO: Zwróć DTO
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.AppointmentId }, appointment);
        }

        // DELETE: api/Appointments/5 (Anulowanie wizyty - często zmiana statusu)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // TODO: Logika biznesowa - czy można anulować? Kiedy? Jaki status ustawić?
            // Zamiast usuwać, zmień status na 'Cancelled'
            if (appointment.Status == "Scheduled") // Tylko zaplanowane można anulować?
            {
                appointment.Status = "Cancelled";
                _context.Entry(appointment).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            else if (appointment.Status == "Cancelled")
            {
                // Już anulowana, nic nie rób
            }
            else
            {
                // Nie można anulować wizyty w innym statusie?
                return BadRequest($"Cannot cancel appointment with status '{appointment.Status}'.");
            }


            return NoContent();

            /* // Kod dla fizycznego usuwania (jeśli potrzebne):
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return NoContent();
            */
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}