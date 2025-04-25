using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PainterPalApi.Data;
using PainterPalApi.Models;
namespace PainterPalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Basic authorization for all endpoints
    public class MaterialsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MaterialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Materials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Material>>> GetMaterials()
        {
            return await _context.Materials.ToListAsync();
        }

        // GET: api/Materials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Material>> GetMaterial(int id)
        {
            var material = await _context.Materials.FindAsync(id);

            if (material == null)
            {
                return NotFound();
            }

            return material;
        }

        // GET: api/Materials/category/{category}
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Material>>> GetMaterialsByCategory(string category)
        {
            return await _context.Materials
                .Where(m => m.Category == category)
                .ToListAsync();
        }

        // POST: api/Materials
        [HttpPost]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can create materials
        public async Task<ActionResult<Material>> CreateMaterial(Material material)
        {
            // Check if material with same name already exists
            if (await _context.Materials.AnyAsync(m => m.MaterialName == material.MaterialName))
            {
                return BadRequest("A material with this name already exists");
            }

            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMaterial), new { id = material.Id }, material);
        }

        // PUT: api/Materials/5
        [HttpPut("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can update materials
        public async Task<IActionResult> UpdateMaterial(int id, Material material)
        {
            if (id != material.Id)
            {
                return BadRequest();
            }

            // Check for name uniqueness (excluding current material)
            if (await _context.Materials.AnyAsync(m =>
                m.MaterialName == material.MaterialName && m.Id != id))
            {
                return BadRequest("A material with this name already exists");
            }

            _context.Entry(material).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialExists(id))
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

        // PUT: api/Materials/5/inventory
        [HttpPut("{id}/inventory")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can update inventory
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] InventoryUpdateModel model)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Materials/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can delete materials
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            // Check if this material is used in any jobs
            var isUsed = await _context.JobMaterials.AnyAsync(jm => jm.MaterialId == id);
            if (isUsed)
            {
                return BadRequest("This material is being used in jobs and cannot be deleted");
            }

            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.Id == id);
        }
    }

    // Model for inventory update requests
    public class InventoryUpdateModel
    {
        public int Quantity { get; set; }
    }
}
