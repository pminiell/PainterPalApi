using PainterPalApi.DTOs;

namespace PainterPalApi.Interfaces
{
    public interface IMaterialService
    {
        Task<IEnumerable<MaterialDTO>> GetMaterialsAsync();
        Task<MaterialDTO> GetMaterialByIdAsync(int id);
        Task<MaterialDTO> CreateMaterialAsync(MaterialDTO materialDto);
        Task<MaterialDTO> UpdateMaterialAsync(int id, MaterialDTO materialDto);
        Task<bool> DeleteMaterialAsync(int id);
    }
}
