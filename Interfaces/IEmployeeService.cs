using PainterPalApi.DTOs;

namespace PainterPalApi.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetEmployeesAsync();
        Task<EmployeeDTO> GetEmployeeByIdAsync(int id);
        Task<EmployeeDTO> CreateEmployeeAsync(EmployeeDTO employeeDto);
        Task<EmployeeDTO> UpdateEmployeeAsync(int id, EmployeeDTO employeeDto);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
