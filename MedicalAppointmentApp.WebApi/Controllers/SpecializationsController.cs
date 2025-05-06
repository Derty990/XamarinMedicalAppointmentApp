using MedicalAppointmentApp.WebApi.Data;
using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.ForView; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using MedicalAppointmentApp.WebApi.Helpers;

namespace MedicalAppointmentApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        

        public SpecializationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Specializations
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SpecializationForView>))] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<SpecializationForView>>> GetSpecializations()
        {
            var specializations = await _context.Specializations.ToListAsync();
         
            var specializationsForView = specializations.Select(s => (SpecializationForView)s).ToList();
            return Ok(specializationsForView);
        }

        // GET: api/Specializations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecializationForView>> GetSpecialization(int id)
        {
            var specialization = await _context.Specializations.FindAsync(id);

            if (specialization == null)
            {
                return NotFound();
            }

            SpecializationForView specializationForView = specialization; 
            return Ok(specializationForView);
        }

        // POST: api/Specializations
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SpecializationForView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<SpecializationForView>> PostSpecialization(SpecializationForView specializationForView)
        {
          
            Specialization specialization = specializationForView; 
            if (specialization == null)
            {
                // Jeśli konwersja zawiedzie lub DTO jest puste
                return BadRequest("Invalid input data.");
            }
           
            specialization.SpecializationId = 0;

            _context.Specializations.Add(specialization);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                Console.WriteLine($"DbUpdateException creating specialization: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error creating specialization. Name might already exist.");
            }

            SpecializationForView createdForView = specialization; // Konwersja zapisanej encji do ForView
            return CreatedAtAction(nameof(GetSpecialization), new { id = specialization.SpecializationId }, createdForView);
        }

        // PUT: api/Specializations/5
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecialization(int id, SpecializationForView specializationForView)
        {
           
            var specializationToUpdate = await _context.Specializations.FindAsync(id);
            if (specializationToUpdate == null)
            {
                return NotFound();
            }

           
            specializationToUpdate.CopyProperties(specializationForView);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SpecializationExists(id)) { return NotFound(); } else { throw; }
            }
            catch (DbUpdateException ex) // Uproszczona obsługa
            {
                Console.WriteLine($"DbUpdateException updating specialization {id}: {ex.InnerException?.Message ?? ex.Message}");
                return Conflict("Database error updating specialization. Name might already exist.");
            }

            return NoContent();
        }

        // DELETE: api/Specializations/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteSpecialization(int id)
        {
            var specialization = await _context.Specializations.FindAsync(id);
            if (specialization == null)
            {
                return NotFound();
            }

            _context.Specializations.Remove(specialization);
            await _context.SaveChangesAsync(); 

            return NoContent();
        }

        private async Task<bool> SpecializationExists(int id)
        {
            return await _context.Specializations.AnyAsync(e => e.SpecializationId == id);
        }
    }
}