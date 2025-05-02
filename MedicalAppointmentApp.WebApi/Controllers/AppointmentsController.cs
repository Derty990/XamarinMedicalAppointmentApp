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
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Definicja DTO do aktualizacji (może być w osobnym pliku w Dtos)
        public class AppointmentUpdateDto
        {
            [System.ComponentModel.DataAnnotations.Required]
            public int StatusId { get; set; }
            [System.ComponentModel.DataAnnotations.Required]
            public System.DateTime AppointmentDate { get; set; }
            [System.ComponentModel.DataAnnotations.Required]
            public System.TimeSpan StartTime { get; set; }
            [System.ComponentModel.DataAnnotations.Required]
            public System.TimeSpan EndTime { get; set; }
        }


        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentForView>>> GetAppointments()
        {
            
            var appointments = await _context.Appointments
                                          .Include(a => a.Patient)
                                          .Include(a => a.Doctor)
                                              .ThenInclude(d => d.User) // Potrzebujemy Usera z Lekarza
                                          .Include(a => a.Clinic)
                                          .Include(a => a.Status)
                                          .ToListAsync();

            // Konwersja listy encji na listę ForView za pomocą operatora
            return Ok(appointments.Select(a => (AppointmentForView)a).ToList());
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentForView>> GetAppointment(int id)
        {
           
            var appointment = await _context.Appointments
                                         .Include(a => a.Patient)
                                         .Include(a => a.Doctor)
                                             .ThenInclude(d => d.User)
                                         .Include(a => a.Clinic)
                                         .Include(a => a.Status)
                                         .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null) return NotFound();

            AppointmentForView forView = appointment; // Konwersja
            return Ok(forView);
        }

        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult<AppointmentForView>> PostAppointment(AppointmentCreateDto appointmentCreateDto) 
        {
            
            if (!await _context.Users.AnyAsync(u => u.UserId == appointmentCreateDto.PatientId)) return BadRequest($"Invalid PatientId: User {appointmentCreateDto.PatientId} not found.");
            if (!await _context.Doctors.AnyAsync(d => d.DoctorId == appointmentCreateDto.DoctorId)) return BadRequest($"Invalid DoctorId: Doctor {appointmentCreateDto.DoctorId} not found.");
            if (!await _context.Clinics.AnyAsync(c => c.ClinicId == appointmentCreateDto.ClinicId)) return BadRequest($"Invalid ClinicId: Clinic {appointmentCreateDto.ClinicId} not found.");
            if (!await _context.AppointmentStatuses.AnyAsync(s => s.StatusId == appointmentCreateDto.StatusId)) return BadRequest($"Invalid StatusId: Status {appointmentCreateDto.StatusId} not found.");

            var appointment = new Appointment();
            appointment.CopyProperties(appointmentCreateDto); 

            _context.Appointments.Add(appointment);

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException creating appointment: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error creating appointment. Referenced entities might be invalid.");
            }

            // Pobierz ponownie z Include, aby zwrócić pełne ForView
            var createdAppointmentWithData = await _context.Appointments
                                          .Include(a => a.Patient)
                                          .Include(a => a.Doctor.User)
                                          .Include(a => a.Clinic)
                                          .Include(a => a.Status)
                                          .FirstOrDefaultAsync(a => a.AppointmentId == appointment.AppointmentId);

            if (createdAppointmentWithData == null)
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not retrieve created appointment data.");

            AppointmentForView createdForView = createdAppointmentWithData; // Konwersja

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.AppointmentId }, createdForView);
        }

        // PUT: api/Appointments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, AppointmentUpdateDto appointmentUpdateDto) // Przyjmuje UpdateDto
        {
            var appointmentToUpdate = await _context.Appointments.FindAsync(id);
            if (appointmentToUpdate == null) return NotFound();

           
            if (appointmentToUpdate.StatusId != appointmentUpdateDto.StatusId &&
                !await _context.AppointmentStatuses.AnyAsync(s => s.StatusId == appointmentUpdateDto.StatusId))
            {
                return BadRequest($"Invalid StatusId: Status {appointmentUpdateDto.StatusId} not found.");
            }
            // TODO: Inne walidacje (np. czy data/godzina są poprawne)

            // Mapuj tylko dozwolone pola
            appointmentToUpdate.CopyProperties(appointmentUpdateDto);

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AppointmentExists(id)) { return NotFound(); } else { throw; }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException updating appointment {id}: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error updating appointment. Referenced status might be invalid.");
            }

            return NoContent();
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            _context.Appointments.Remove(appointment);

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException ex)
            { // Na wszelki wypadek
                Console.WriteLine($"DbUpdateException deleting appointment {id}: {ex.InnerException?.Message ?? ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting appointment.");
            }

            return NoContent();
        }

        private async Task<bool> AppointmentExists(int id)
        {
            return await _context.Appointments.AnyAsync(e => e.AppointmentId == id);
        }
    }
}