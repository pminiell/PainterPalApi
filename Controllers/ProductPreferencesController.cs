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
    public class ProductPreferencesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductPreferencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ProductPreferences
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductPreferences>>> GetProductPreferences()
        {
            return await _context.ProductPreferences
                .Include(p => p.Colour)
                .ToListAsync();
        }

        // GET: api/ProductPreferences/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductPreferences>> GetProductPreference(int id)
        {
            var productPreference = await _context.ProductPreferences
                .Include(p => p.Colour)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productPreference == null)
            {
                return NotFound();
            }

            return productPreference;
        }

        // POST: api/ProductPreferences
        [HttpPost]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can create product preferences
        public async Task<ActionResult<ProductPreferences>> CreateProductPreference(ProductPreferences productPreference)
        {
            // Validate the colour exists if a colour ID is provided
            if (productPreference.PreferredColourId != 0 && !await _context.Colours.AnyAsync(c => c.Id == productPreference.PreferredColourId))
            {
                return BadRequest("The specified colour does not exist");
            }

            // Check if product with same name already exists
            if (await _context.ProductPreferences.AnyAsync(p => p.ProductName == productPreference.ProductName))
            {
                return BadRequest("A product with this name already exists");
            }

            _context.ProductPreferences.Add(productPreference);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductPreference), new { id = productPreference.Id }, productPreference);
        }

        // PUT: api/ProductPreferences/5
        [HttpPut("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can update product preferences
        public async Task<IActionResult> UpdateProductPreference(int id, ProductPreferences productPreference)
        {
            if (id != productPreference.Id)
            {
                return BadRequest();
            }

            // Validate the colour exists if a colour ID is provided
            if (productPreference.PreferredColourId != 0 && !await _context.Colours.AnyAsync(c => c.Id == productPreference.PreferredColourId))
            {
                return BadRequest("The specified colour does not exist");
            }

            // Check for name uniqueness (excluding current product)
            if (await _context.ProductPreferences.AnyAsync(p => 
                p.ProductName == productPreference.ProductName && p.Id != id))
            {
                return BadRequest("A product with this name already exists");
            }

            _context.Entry(productPreference).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductPreferenceExists(id))
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

        // DELETE: api/ProductPreferences/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can delete product preferences
        public async Task<IActionResult> DeleteProductPreference(int id)
        {
            var productPreference = await _context.ProductPreferences.FindAsync(id);
            if (productPreference == null)
            {
                return NotFound();
            }

            // Check if this product is being used in any jobs before deleting
            // This assumes you have a relationship between jobs and product preferences
            var isUsed = await _context.Jobs.AnyAsync(j => j.ProductPreferenceId == id);
            if (isUsed)
            {
                return BadRequest("This product is being used in jobs and cannot be deleted");
            }

            _context.ProductPreferences.Remove(productPreference);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductPreferenceExists(int id)
        {
            return _context.ProductPreferences.Any(e => e.Id == id);
        }
    }
}