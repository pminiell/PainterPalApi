using AutoMapper;
using PainterPalApi.DTOs;
using PainterPalApi.Models;

namespace PainterPalApi.Profiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<User, EmployeeDTO>();
            CreateMap<EmployeeDTO, User>();
        }
    }
}
