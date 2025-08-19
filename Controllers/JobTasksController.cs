using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PainterPalApi.DTOs;
using PainterPalApi.Interfaces;

namespace PainterPalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobTasksController : ControllerBase
    {
        private readonly IJobTaskService _jobTaskService;

        public JobTasksController(IJobTaskService jobTaskService)
        {
            _jobTaskService = jobTaskService;
        }

        // GET: api/JobTasks/job/{jobId}
        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<IEnumerable<JobTaskDTO>>> GetTasksByJob(int jobId)
        {
            var tasks = await _jobTaskService.GetJobTasksByJobIdAsync(jobId);
            return Ok(tasks);
        }

        // GET: api/JobTasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<JobTaskDTO>> GetTask(int id)
        {
            var task = await _jobTaskService.GetJobTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        // POST: api/JobTasks/job/{jobId}
        [HttpPost("job/{jobId}")]
        public async Task<ActionResult<JobTaskDTO>> CreateTask(int jobId, JobTaskDTO jobTaskDto)
        {
            jobTaskDto.JobId = jobId; // Ensure the JobId is set from the route
            var createdTask = await _jobTaskService.CreateJobTaskAsync(jobTaskDto);
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
        }

        // PUT: api/JobTasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, JobTaskDTO jobTaskDto)
        {
            if (id != jobTaskDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var updatedTask = await _jobTaskService.UpdateJobTaskAsync(id, jobTaskDto);
            if (updatedTask == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/JobTasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _jobTaskService.DeleteJobTaskAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        // PATCH: api/JobTasks/{id}/complete
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var result = await _jobTaskService.CompleteJobTaskAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        // PATCH: api/JobTasks/{id}/uncomplete
        [HttpPatch("{id}/uncomplete")]
        public async Task<IActionResult> UncompleteTask(int id)
        {
            var result = await _jobTaskService.UncompleteJobTaskAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
