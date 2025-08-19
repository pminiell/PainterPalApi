using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PainterPalApi.DTOs;
using PainterPalApi.Interfaces;
using PainterPalApi.Models;

namespace PainterPalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Basic authorization for all endpoints
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobDTO>>> GetJobs([FromQuery] JobStatus? status = null)
        {
            var jobs = await _jobService.GetJobsAsync(status);
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobDTO>> GetJob(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }

        [HttpPost]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can create jobs
        public async Task<ActionResult<JobDTO>> CreateJob(JobDTO jobDto)
        {
            var createdJob = await _jobService.CreateJobAsync(jobDto);
            return CreatedAtAction(nameof(GetJob), new { id = createdJob.Id }, createdJob);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can update jobs
        public async Task<IActionResult> UpdateJob(int id, JobDTO jobDto)
        {
            if (id != jobDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var updatedJob = await _jobService.UpdateJobAsync(id, jobDto);
            if (updatedJob == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateJobStatus(int id, [FromBody] JobStatusUpdateModel model)
        {
            var result = await _jobService.UpdateJobStatusAsync(id, model.Status);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can delete jobs
        public async Task<IActionResult> DeleteJob(int id)
        {
            var result = await _jobService.DeleteJobAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("{id}/employees")]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetJobEmployees(int id)
        {
            var employees = await _jobService.GetJobEmployeesAsync(id);
            return Ok(employees);
        }

        [HttpPost("{id}/employees")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can assign employees
        public async Task<IActionResult> AssignEmployeeToJob(int id, [FromBody] EmployeeAssignmentModel model)
        {
            var result = await _jobService.AssignEmployeeToJobAsync(id, model.EmployeeId);
            if (!result)
            {
                return BadRequest("Failed to assign employee or already assigned.");
            }
            return Ok();
        }

        [HttpDelete("{id}/employees/{employeeId}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can remove employees
        public async Task<IActionResult> RemoveEmployeeFromJob(int id, int employeeId)
        {
            var result = await _jobService.RemoveEmployeeFromJobAsync(id, employeeId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

    public class JobStatusUpdateModel
    {
        public JobStatus Status { get; set; }
    }

    public class EmployeeAssignmentModel
    {
        public int EmployeeId { get; set; }
    }
}

