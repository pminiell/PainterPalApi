using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PainterPalApi.Data;
using PainterPalApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PainterPalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Basic authorization for all endpoints
    public class ColoursController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ColoursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Colours
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Colour>>> GetColours()
        {
            return await _context.Colours.ToListAsync();
        }

        // GET: api/Colours/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Colour>> GetColour(int id)
        {
            var colour = await _context.Colours.FindAsync(id);

            if (colour == null)
            {
                return NotFound();
            }

            return colour;
        }

        // GET: api/Colours/search?name={name}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Colour>>> SearchColours([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await _context.Colours.ToListAsync();
            }

            return await _context.Colours
                .Where(c => c.ColourName.Contains(name.ToLower()))
                .ToListAsync();
        }

        // POST: api/Colours
        [HttpPost]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can create colours
        public async Task<ActionResult<Colour>> CreateColour(Colour colour)
        {
            // Check if colour with same name already exists
            if (await _context.Colours.AnyAsync(c => c.ColourName == colour.ColourName))
            {
                return BadRequest("A colour with this name already exists");
            }

            // Check if colour with same code already exists
            if (await _context.Colours.AnyAsync(c => c.ColourCode == colour.ColourCode))
            {
                return BadRequest("A colour with this code already exists");
            }

            _context.Colours.Add(colour);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetColour), new { id = colour.Id }, colour);
        }

        // PUT: api/Colours/5
        [HttpPut("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can update colours
        public async Task<IActionResult> UpdateColour(int id, Colour colour)
        {
            if (id != colour.Id)
            {
                return BadRequest();
            }

            // Check for name uniqueness (excluding current colour)
            if (await _context.Colours.AnyAsync(c => 
                c.ColourName == colour.ColourName && c.Id != id))
            {
                return BadRequest("A colour with this name already exists");
            }

            // Check for code uniqueness (excluding current colour)
            if (await _context.Colours.AnyAsync(c => 
                c.ColourCode == colour.ColourCode && c.Id != id))
            {
                return BadRequest("A colour with this code already exists");
            }

            _context.Entry(colour).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ColourExists(id))
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

        // DELETE: api/Colours/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can delete colours
        public async Task<IActionResult> DeleteColour(int id)
        {
            var colour = await _context.Colours.FindAsync(id);
            if (colour == null)
            {
                return NotFound();
            }

            // Check if colour is used in any product preferences before deleting
            var isUsed = await _context.UserPreferredColours.AnyAsync(pp => pp.ColourId == id);
            if (isUsed)
            {
                return BadRequest("This colour is being used in product preferences and cannot be deleted");
            }

            _context.Colours.Remove(colour);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ColourExists(int id)
        {
            return _context.Colours.Any(e => e.Id == id);
        }
    }
}