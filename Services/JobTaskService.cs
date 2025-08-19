using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PainterPalApi.Data;
using PainterPalApi.DTOs;
using PainterPalApi.Interfaces;
using PainterPalApi.Models;

namespace PainterPalApi.Services
{
    public class JobTaskService : IJobTaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public JobTaskService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<JobTaskDTO>> GetJobTasksByJobIdAsync(int jobId)
        {
            var tasks = await _context.JobTasks
                .Where(t => t.JobId == jobId)
                .OrderBy(t => t.Priority)
                .ThenBy(t => t.CreatedAt)
                .ToListAsync();
            return _mapper.Map<IEnumerable<JobTaskDTO>>(tasks);
        }

        public async Task<JobTaskDTO> GetJobTaskByIdAsync(int id)
        {
            var task = await _context.JobTasks.FindAsync(id);
            return _mapper.Map<JobTaskDTO>(task);
        }

        public async Task<JobTaskDTO> CreateJobTaskAsync(JobTaskDTO jobTaskDto)
        {
            var task = _mapper.Map<JobTask>(jobTaskDto);
            _context.JobTasks.Add(task);
            await _context.SaveChangesAsync();
            return _mapper.Map<JobTaskDTO>(task);
        }

        public async Task<JobTaskDTO> UpdateJobTaskAsync(int id, JobTaskDTO jobTaskDto)
        {
            var task = await _context.JobTasks.FindAsync(id);
            if (task == null)
            {
                return null;
            }

            _mapper.Map(jobTaskDto, task);
            await _context.SaveChangesAsync();
            return _mapper.Map<JobTaskDTO>(task);
        }

        public async Task<bool> DeleteJobTaskAsync(int id)
        {
            var task = await _context.JobTasks.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            _context.JobTasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteJobTaskAsync(int id)
        {
            var task = await _context.JobTasks.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            task.IsCompleted = true;
            task.CompletedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UncompleteJobTaskAsync(int id)
        {
            var task = await _context.JobTasks.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            task.IsCompleted = false;
            task.CompletedAt = null;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
