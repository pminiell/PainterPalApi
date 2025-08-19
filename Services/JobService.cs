using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PainterPalApi.Data;
using PainterPalApi.DTOs;
using PainterPalApi.Interfaces;
using PainterPalApi.Models;

namespace PainterPalApi.Services
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public JobService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<JobDTO>> GetJobsAsync(JobStatus? status = null)
        {
            var query = _context.Jobs
                .Include(j => j.Customer)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(j => j.JobStatus == status.Value);
            }

            var jobs = await query.ToListAsync();
            return _mapper.Map<IEnumerable<JobDTO>>(jobs);
        }

        public async Task<JobDTO> GetJobByIdAsync(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Customer)
                .Include(j => j.PreferredColours)
                .Include(j => j.JobTaskList)
                .FirstOrDefaultAsync(j => j.Id == id);

            return _mapper.Map<JobDTO>(job);
        }

        public async Task<JobDTO> CreateJobAsync(JobDTO jobDto)
        {
            var job = _mapper.Map<Job>(jobDto);
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return _mapper.Map<JobDTO>(job);
        }

        public async Task<JobDTO> UpdateJobAsync(int id, JobDTO jobDto)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return null;
            }

            _mapper.Map(jobDto, job);
            await _context.SaveChangesAsync();
            return _mapper.Map<JobDTO>(job);
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return false;
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetJobEmployeesAsync(int jobId)
        {
            var employees = await _context.JobEmployees
                .Where(je => je.JobId == jobId)
                .Select(je => je.Employee)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EmployeeDTO>>(employees);
        }

        public async Task<bool> AssignEmployeeToJobAsync(int jobId, int employeeId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            var employee = await _context.Users.FindAsync(employeeId);

            if (job == null || employee == null)
            {
                return false;
            }

            var existingAssignment = await _context.JobEmployees
                .FirstOrDefaultAsync(je => je.JobId == jobId && je.EmployeeId == employeeId);

            if (existingAssignment != null)
            {
                return false; // Already assigned
            }

            var assignment = new JobEmployee
            {
                JobId = jobId,
                EmployeeId = employeeId,
                AssignedDate = DateTime.UtcNow
            };

            _context.JobEmployees.Add(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveEmployeeFromJobAsync(int jobId, int employeeId)
        {
            var assignment = await _context.JobEmployees
                .FirstOrDefaultAsync(je => je.JobId == jobId && je.EmployeeId == employeeId);

            if (assignment == null)
            {
                return false;
            }

            _context.JobEmployees.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateJobStatusAsync(int id, JobStatus status)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return false;
            }

            job.JobStatus = status;
            if (status == JobStatus.Completed)
            {
                job.CompletionDate = DateTime.UtcNow;
            }
            else
            {
                job.CompletionDate = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
