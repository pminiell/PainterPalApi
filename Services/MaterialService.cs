using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PainterPalApi.Data;
using PainterPalApi.DTOs;
using PainterPalApi.Interfaces;
using PainterPalApi.Models;

namespace PainterPalApi.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MaterialService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MaterialDTO>> GetMaterialsAsync()
        {
            var materials = await _context.Materials.ToListAsync();
            return _mapper.Map<IEnumerable<MaterialDTO>>(materials);
        }

        public async Task<MaterialDTO> GetMaterialByIdAsync(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            return _mapper.Map<MaterialDTO>(material);
        }

        public async Task<MaterialDTO> CreateMaterialAsync(MaterialDTO materialDto)
        {
            var material = _mapper.Map<Material>(materialDto);
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();
            return _mapper.Map<MaterialDTO>(material);
        }

        public async Task<MaterialDTO> UpdateMaterialAsync(int id, MaterialDTO materialDto)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return null;
            }

            _mapper.Map(materialDto, material);
            await _context.SaveChangesAsync();
            return _mapper.Map<MaterialDTO>(material);
        }

        public async Task<bool> DeleteMaterialAsync(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return false;
            }

            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
