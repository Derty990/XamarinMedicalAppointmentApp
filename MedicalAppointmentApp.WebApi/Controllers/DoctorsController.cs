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
    public class DoctorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Doctors - Zwraca listę DoctorListItemDto (ręczne mapowanie)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorListItemDto>>> GetDoctors()
        {
           
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .Select(d => new DoctorListItemDto // Tworzymy ręcznie
                {
                    DoctorId = d.DoctorId,
                    FullName = d.User != null ? $"{d.User.FirstName} {d.User.LastName}" : "N/A",
                    SpecializationName = d.Specialization != null ? d.Specialization.Name : "N/A"
                })
                .ToListAsync();
        }

        // GET: api/Doctors/5 - Zwraca DoctorForView
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorForView>> GetDoctor(int id)
        {
           
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (doctor == null) return NotFound();

            DoctorForView forView = doctor; 
            return Ok(forView);
        }

        // POST: api/Doctors - Przyjmuje DoctorCreateDto, zwraca DoctorForView
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DoctorForView))] // Deklarujemy 201
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DoctorForView>> PostDoctor(DoctorCreateDto doctorCreateDto)
        {
            
            if (!await _context.Users.AnyAsync(u => u.UserId == doctorCreateDto.UserId)) return BadRequest($"Invalid UserId: User {doctorCreateDto.UserId} not found.");
            if (!await _context.Specializations.AnyAsync(s => s.SpecializationId == doctorCreateDto.SpecializationId)) return BadRequest($"Invalid SpecializationId: Specialization {doctorCreateDto.SpecializationId} not found.");
            if (await _context.Doctors.AnyAsync(d => d.UserId == doctorCreateDto.UserId)) return Conflict($"User {doctorCreateDto.UserId} is already a Doctor.");

            var doctor = new Doctor();
            doctor.CopyProperties(doctorCreateDto); // Kopiuje UserId, SpecializationId

            _context.Doctors.Add(doctor);

            
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException creating doctor: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error creating doctor. User might already be a doctor or User/Specialization ID is invalid.");
            }

           
            var createdDoctorWithData = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.DoctorId == doctor.DoctorId);

            if (createdDoctorWithData == null) return StatusCode(StatusCodes.Status500InternalServerError, "Could not retrieve created doctor data.");

            DoctorForView createdForView = createdDoctorWithData; 

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.DoctorId }, createdForView);
        }

        // PUT: api/Doctors/5 - Przyjmuje DoctorUpdateDto (tylko SpecializationId)
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Sukces
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Błąd walidacji
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Nie znaleziono lekarza
        public async Task<IActionResult> PutDoctor(int id, DoctorUpdateDto doctorUpdateDto)
        {
            var doctorToUpdate = await _context.Doctors.FindAsync(id);
            if (doctorToUpdate == null) return NotFound();

           
            if (doctorToUpdate.SpecializationId != doctorUpdateDto.SpecializationId &&
                !await _context.Specializations.AnyAsync(s => s.SpecializationId == doctorUpdateDto.SpecializationId))
            {
                return BadRequest($"Invalid SpecializationId: Specialization {doctorUpdateDto.SpecializationId} not found.");
            }

            doctorToUpdate.SpecializationId = doctorUpdateDto.SpecializationId;

            await _context.SaveChangesAsync();

            return NoContent(); 
        }

        // DELETE: api/Doctors/5 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();
            bool hasAppointments = await _context.Appointments.AnyAsync(a => a.DoctorId == id);
            if (hasAppointments) return Conflict($"Cannot delete Doctor ID {id} because they have associated appointments.");
            _context.Doctors.Remove(doctor);
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException deleting doctor {id}: {ex.InnerException?.Message ?? ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting doctor.");
            }
            return NoContent();
        }

        private async Task<bool> DoctorExists(int id)
        {
            return await _context.Doctors.AnyAsync(e => e.DoctorId == id);
        }
    }
}