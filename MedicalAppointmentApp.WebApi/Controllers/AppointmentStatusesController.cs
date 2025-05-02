using MedicalAppointmentApp.WebApi.Data;
using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.ForView; 
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
    public class AppointmentStatusesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentStatusesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AppointmentStatuses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentStatusForView>>> GetAppointmentStatuses()
        {
            var statuses = await _context.AppointmentStatuses.ToListAsync();
            return Ok(statuses.Select(s => (AppointmentStatusForView)s).ToList());
        }

        // GET: api/AppointmentStatuses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentStatusForView>> GetAppointmentStatus(int id)
        {
            var status = await _context.AppointmentStatuses.FindAsync(id);
            if (status == null) return NotFound();
            AppointmentStatusForView forView = status;
            return Ok(forView);
        }

        // POST: api/AppointmentStatuses - Rzadko używane
        [HttpPost]
        public async Task<ActionResult<AppointmentStatusForView>> PostAppointmentStatus(AppointmentStatusForView statusForView)
        {
            AppointmentStatus status = statusForView; 
            if (status == null) return BadRequest("Invalid input.");
            status.StatusId = 0; 

            _context.AppointmentStatuses.Add(status);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                Console.WriteLine($"DbUpdateException creating status: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error creating status. Name might already exist.");
            }

            AppointmentStatusForView createdForView = status;
            return CreatedAtAction(nameof(GetAppointmentStatus), new { id = status.StatusId }, createdForView);
        }

        // PUT: api/AppointmentStatuses/5 - Rzadko używane
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointmentStatus(int id, AppointmentStatusForView statusForView)
        {
            var statusToUpdate = await _context.AppointmentStatuses.FindAsync(id);
            if (statusToUpdate == null) return NotFound();

            statusToUpdate.CopyProperties(statusForView);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AppointmentStatusExists(id)) { return NotFound(); } else { throw; }
            }
            catch (DbUpdateException ex) 
            {
                Console.WriteLine($"DbUpdateException updating status {id}: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error updating status. Name might already exist.");
            }

            return NoContent();
        }

        // DELETE: api/AppointmentStatuses/5 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointmentStatus(int id)
        {
            var status = await _context.AppointmentStatuses.FindAsync(id);
            if (status == null) return NotFound();

            _context.AppointmentStatuses.Remove(status);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                Console.WriteLine($"DbUpdateException deleting status {id}: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict($"Cannot delete Status ID {id} because it might be in use by Appointments.");
            }

            return NoContent();
        }

        private async Task<bool> AppointmentStatusExists(int id)
        {
            return await _context.AppointmentStatuses.AnyAsync(e => e.StatusId == id);
        }
    }
}