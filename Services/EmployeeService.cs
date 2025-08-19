using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PainterPalApi.Data;
using PainterPalApi.DTOs;
using PainterPalApi.Interfaces;
using PainterPalApi.Models;

namespace PainterPalApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetEmployeesAsync()
        {
            var users = await _context.Users.Where(u => u.Role == "Employee").ToListAsync();
            return _mapper.Map<IEnumerable<EmployeeDTO>>(users);
        }

        public async Task<EmployeeDTO> GetEmployeeByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return _mapper.Map<EmployeeDTO>(user);
        }

        public async Task<EmployeeDTO> CreateEmployeeAsync(EmployeeDTO employeeDto)
        {
            var user = _mapper.Map<User>(employeeDto);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return _mapper.Map<EmployeeDTO>(user);
        }

        public async Task<EmployeeDTO> UpdateEmployeeAsync(int id, EmployeeDTO employeeDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            _mapper.Map(employeeDto, user);
            await _context.SaveChangesAsync();
            return _mapper.Map<EmployeeDTO>(user);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
