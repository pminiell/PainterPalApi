using AutoMapper;
using PainterPalApi.DTOs;
using PainterPalApi.Models;

namespace PainterPalApi.Profiles
{
    public class MaterialProfile : Profile
    {
        public MaterialProfile()
        {
            CreateMap<Material, MaterialDTO>();
            CreateMap<MaterialDTO, Material>();
        }
    }
}
