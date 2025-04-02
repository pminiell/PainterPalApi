using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PainterPalApi.Data;
using PainterPalApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PainterPalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Basic authorization for all endpoints
    public class JobsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs([FromQuery] string status = null)
        {
            var query = _context.Jobs.AsQueryable();
            
            // Filter by status if provided
            if (Enum.TryParse<JobStatus>(status, out var jobStatus))
            {
                query = query.Where(j => j.JobStatus == jobStatus);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Customer) // Include related customer data
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            return job;
        }

        [HttpPost]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can create jobs
        public async Task<ActionResult<Job>> CreateJob(Job job)
        {
            // Set creation date
            job.CreatedAt = DateTime.Now;
            
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can update jobs
        public async Task<IActionResult> UpdateJob(int id, Job job)
        {
            if (id != job.Id)
            {
                return BadRequest();
            }

            _context.Entry(job).State = EntityState.Modified;
            
            // Don't modify creation date
            _context.Entry(job).Property(x => x.CreatedAt).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(id))
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

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateJobStatus(int id, [FromBody] StatusUpdateModel model)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            job.JobStatus = model.Status;
            
            // If status is "Completed", set completion date
            if (model.Status.Equals("Completed"))
            { 
                job.CompletionDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can delete jobs
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/employees")]
        public async Task<ActionResult<IEnumerable<User>>> GetJobEmployees(int id)
        {
            // This assumes you have a JobEmployee join table
            var employees = await _context.Users
                .Where(u => u.JobAssignments.Any(ja => ja.JobId == id))
                .ToListAsync();

            return employees;
        }

        [HttpPost("{id}/employees")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can assign employees
        public async Task<IActionResult> AssignEmployeeToJob(int id, [FromBody] EmployeeAssignmentModel model)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound("Job not found");
            }

            var employee = await _context.Users.FindAsync(model.EmployeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            // Check if the assignment already exists
            var existingAssignment = await _context.JobEmployees
                .FirstOrDefaultAsync(je => je.JobId == id && je.EmployeeId == model.EmployeeId);

            if (existingAssignment != null)
            {
                return BadRequest("Employee is already assigned to this job");
            }

            var assignment = new JobEmployee
            {
                JobId = id,
                EmployeeId = model.EmployeeId,
                AssignedDate = DateTime.Now
            };

            _context.JobEmployees.Add(assignment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}/employees/{employeeId}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can remove employees
        public async Task<IActionResult> RemoveEmployeeFromJob(int id, int employeeId)
        {
            var assignment = await _context.JobEmployees
                .FirstOrDefaultAsync(je => je.JobId == id && je.EmployeeId == employeeId);

            if (assignment == null)
            {
                return NotFound("Employee is not assigned to this job");
            }

            _context.JobEmployees.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
    }

    // Models for job-related requests
    public class StatusUpdateModel
    {
        public JobStatus Status { get; set; }
    }

    public class EmployeeAssignmentModel
    {
        public int EmployeeId { get; set; }
    } 
}