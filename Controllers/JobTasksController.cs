// /home/pminiell/Documents/Coding/Dotnet/PainterPal/PainterPalApi/Controllers/JobTasksController.cs
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
    [Authorize]
    public class JobTasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JobTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/JobTasks/job/{jobId}
        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<IEnumerable<JobTaskDTO>>> GetTasksByJob(int jobId)
        {
            var tasks = await _context.JobTasks
                .Where(t => t.JobId == jobId)
                .OrderBy(t => t.Priority)
                .ThenBy(t => t.CreatedAt)
                .ToListAsync();

            return tasks.Select(t => new JobTaskDTO
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
            }).ToList();
        }

        // GET: api/JobTasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<JobTaskDTO>> GetTask(int id)
        {
            var task = await _context.JobTasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return new JobTaskDTO
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                Priority = task.Priority,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                CompletedAt = task.CompletedAt,
                JobId = task.JobId
            };
        }

        // POST: api/JobTasks/job/{jobId}
        [HttpPost("job/{jobId}")]
        public async Task<ActionResult<JobTaskDTO>> CreateTask(int jobId, CreateJobTaskDTO taskDto)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                return NotFound("Job not found");
            }

            var task = new JobTask
            {
                Name = taskDto.Name,
                Description = taskDto.Description,
                Priority = taskDto.Priority,
                JobId = jobId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.JobTasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, new JobTaskDTO
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                Priority = task.Priority,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                CompletedAt = task.CompletedAt,
                JobId = task.JobId
            });
        }

        // PUT: api/JobTasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateJobTaskDTO taskDto)
        {
            var task = await _context.JobTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            task.Name = taskDto.Name;
            task.Description = taskDto.Description;
            task.Priority = taskDto.Priority;
            task.UpdatedAt = DateTime.UtcNow;

            // If completing the task, set CompletedAt timestamp
            if (taskDto.IsCompleted && !task.IsCompleted)
            {
                task.IsCompleted = true;
                task.CompletedAt = DateTime.UtcNow;
            }
            else if (!taskDto.IsCompleted && task.IsCompleted)
            {
                // If un-completing a previously completed task
                task.IsCompleted = false;
                task.CompletedAt = null;
            }

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
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

        // DELETE: api/JobTasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.JobTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.JobTasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/JobTasks/{id}/complete
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var task = await _context.JobTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            task.IsCompleted = true;
            task.CompletedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/JobTasks/{id}/uncomplete
        [HttpPatch("{id}/uncomplete")]
        public async Task<IActionResult> UncompleteTask(int id)
        {
            var task = await _context.JobTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            task.IsCompleted = false;
            task.CompletedAt = null;
            task.UpdatedAt = DateTime.UtcNow;

            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(int id)
        {
            return _context.JobTasks.Any(e => e.Id == id);
        }
    }
}
