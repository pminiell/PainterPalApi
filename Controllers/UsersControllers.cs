using Microsoft.AspNetCore.Mvc;
using PainterPalApi.DTOs;
using PainterPalApi.Interfaces;

namespace PainterPalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public UsersController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetUsers()
        {
            var employees = await _employeeService.GetEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDTO>> GetUser(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDTO>> PostUser(EmployeeDTO employeeDto)
        {
            var createdEmployee = await _employeeService.CreateEmployeeAsync(employeeDto);
            return CreatedAtAction(nameof(GetUser), new { id = createdEmployee.Id }, createdEmployee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, EmployeeDTO employeeDto)
        {
            if (id != employeeDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, employeeDto);
            if (updatedEmployee == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}

