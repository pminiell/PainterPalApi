using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PainterPalApi.Data;
using PainterPalApi.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PainterPalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserPreferencesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserPreferencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserPreferences/materials
        [HttpGet("materials")]
        public async Task<ActionResult<IEnumerable<UserMaterialPreference>>> GetUserMaterialPreferences()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            return await _context.UserMaterialPreferences
                .Where(p => p.UserId == userId)
                .Include(p => p.Material)
                .ToListAsync();
        }
        
        // GET: api/UserPreferences/colours
        [HttpGet("colours")]
        public async Task<ActionResult<IEnumerable<UserPreferredColour>>> GetUserColourPreferences()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            return await _context.UserPreferredColours
                .Where(p => p.UserId == userId)
                .Include(p => p.Colour)
                .ToListAsync();
        }
        
        // POST: api/UserPreferences/materials
        [HttpPost("materials")]
        public async Task<ActionResult<UserMaterialPreference>> AddMaterialPreference(UserMaterialPreference preference)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            preference.UserId = userId;
            
            // Check if material exists
            if (!await _context.Materials.AnyAsync(m => m.Id == preference.MaterialId))
                return BadRequest("Material not found");
                
            // Check for duplicate
            if (await _context.UserMaterialPreferences.AnyAsync(
                p => p.UserId == userId && p.MaterialId == preference.MaterialId))
                return BadRequest("This material is already in your preferences");
                
            _context.UserMaterialPreferences.Add(preference);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetUserMaterialPreferences), new { }, preference);
        }

        // POST: api/UserPreferences/colours
        [HttpPost("colours")]
        public async Task<ActionResult<UserPreferredColour>> AddColourPreference(UserPreferredColour preference)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            preference.UserId = userId;
            
            // Check if colour exists
            if (!await _context.Colours.AnyAsync(c => c.Id == preference.ColourId))
                return BadRequest("Colour not found");
                
            // Check for duplicate
            if (await _context.UserPreferredColours.AnyAsync(
                p => p.UserId == userId && p.ColourId == preference.ColourId))
                return BadRequest("This colour is already in your preferences");
                
            _context.UserPreferredColours.Add(preference);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetUserColourPreferences), new { }, preference);
        }

        // DELETE: api/UserPreferences/materials/{id}
        [HttpDelete("materials/{id}")]
        public async Task<IActionResult> RemoveMaterialPreference(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var preference = await _context.UserMaterialPreferences
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
                
            if (preference == null)
                return NotFound("Material preference not found");
                
            _context.UserMaterialPreferences.Remove(preference);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        // DELETE: api/UserPreferences/colours/{id}
        [HttpDelete("colours/{id}")]
        public async Task<IActionResult> RemoveColourPreference(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var preference = await _context.UserPreferredColours
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
                
            if (preference == null)
                return NotFound("Colour preference not found");
                
            _context.UserPreferredColours.Remove(preference);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}