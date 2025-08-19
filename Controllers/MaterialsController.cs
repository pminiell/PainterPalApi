using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PainterPalApi.DTOs;
using PainterPalApi.Interfaces;

namespace PainterPalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Basic authorization for all endpoints
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialsController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        // GET: api/Materials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialDTO>>> GetMaterials()
        {
            var materials = await _materialService.GetMaterialsAsync();
            return Ok(materials);
        }

        // GET: api/Materials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialDTO>> GetMaterial(int id)
        {
            var material = await _materialService.GetMaterialByIdAsync(id);
            if (material == null)
            {
                return NotFound();
            }
            return Ok(material);
        }

        // POST: api/Materials
        [HttpPost]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can create materials
        public async Task<ActionResult<MaterialDTO>> CreateMaterial(MaterialDTO materialDto)
        {
            var createdMaterial = await _materialService.CreateMaterialAsync(materialDto);
            return CreatedAtAction(nameof(GetMaterial), new { id = createdMaterial.Id }, createdMaterial);
        }

        // PUT: api/Materials/5
        [HttpPut("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can update materials
        public async Task<IActionResult> UpdateMaterial(int id, MaterialDTO materialDto)
        {
            if (id != materialDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var updatedMaterial = await _materialService.UpdateMaterialAsync(id, materialDto);
            if (updatedMaterial == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/Materials/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can delete materials
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            var result = await _materialService.DeleteMaterialAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
