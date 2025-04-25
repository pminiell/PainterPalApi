using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PainterPalApi.Data;
using PainterPalApi.DTOs;
using PainterPalApi.Models;

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
        public async Task<ActionResult<IEnumerable<JobSummaryDTO>>> GetJobs([FromQuery] string status = null)
        {
            var query = _context.Jobs
                .Include(j => j.Customer)
                .AsQueryable();

            if (Enum.TryParse<JobStatus>(status, out var jobStatus))
            {
                query = query.Where(j => j.JobStatus == jobStatus);
            }

            var jobs = await query.ToListAsync();

            // Map to DTO
            var jobDTOs = jobs.Select(j => new JobSummaryDTO
            {
                Id = j.Id,
                JobName = j.JobName,
                JobLocation = j.JobLocation,
                JobStatus = j.JobStatus,
                StartDate = j.StartDate,
                EndDate = j.EndDate,
                CustomerId = j.CustomerId,
                CustomerName = j.Customer?.CustomerName
            }).ToList();

            return jobDTOs;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobDetailDTO>> GetJob(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Customer)
                .Include(j => j.PreferredColours)
                .Include(j => j.JobTaskList)  // Include JobTaskList
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            // Map to DTO
            var jobDTO = new JobDetailDTO
            {
                Id = job.Id,
                JobName = job.JobName,
                JobNotes = job.JobNotes,
                JobLocation = job.JobLocation,
                JobStatus = job.JobStatus,
                Tags = job.Tags,
                // Map JobTaskList to JobTaskDTO list
                JobTaskList = job.JobTaskList.Select(t => new JobTaskDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Priority = t.Priority,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    CompletedAt = t.CompletedAt,
                    JobId = t.JobId
                }).ToList(),
                StartDate = job.StartDate,
                EndDate = job.EndDate,
                CreatedAt = job.CreatedAt,
                UpdatedAt = job.UpdatedAt,
                CompletionDate = job.CompletionDate,
                Customer = job.Customer != null ? new CustomerSummaryDTO
                {
                    Id = job.Customer.Id,
                    CustomerName = job.Customer.CustomerName,
                    CustomerEmail = job.Customer.CustomerEmail,
                    CustomerPhone = job.Customer.CustomerPhone,
                    CustomerAddress = job.Customer.CustomerAddress
                } : null,
                PreferredColours = job.PreferredColours.Select(c => new ColourSummaryDTO
                {
                    Id = c.Id,
                    Name = c.ColourName,
                    Code = c.ColourCode,
                    Usage = c.ColourUsage
                }).ToList()
            };

            return jobDTO;
        }

        [HttpPost]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can create jobs
        //create a jobcreation DTO for only necessary fields
        public async Task<ActionResult<Job>> CreateJob(CreateJobDTO jobDto)
        {

            var job = new Job
            {
                CustomerId = jobDto.CustomerId,
                JobName = jobDto.JobName,
                JobNotes = jobDto.JobNotes ?? string.Empty,
                JobLocation = jobDto.JobLocation,
                StartDate = jobDto.StartDate.ToUniversalTime(),
                EndDate = jobDto.EndDate.ToUniversalTime(),
                Tags = jobDto.Tags ?? new List<string>(), // Initialize to empty list if null
                JobTaskList = jobDto.JobTaskList ?? new List<JobTask>(), // Initialize to empty list if null
                JobStatus = JobStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            if (jobDto.PreferredColourIds?.Count > 0)
            {
                var colours = await _context.Colours
                    .Where(c => jobDto.PreferredColourIds.Contains(c.Id))
                    .ToListAsync();

                foreach (var colour in colours)
                {
                    job.PreferredColours.Add(colour);
                }
            }

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
                job.CompletionDate = DateTime.UtcNow;
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
                AssignedDate = DateTime.UtcNow
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
