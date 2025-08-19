using PainterPalApi.DTOs;
using PainterPalApi.Models;

namespace PainterPalApi.Interfaces
{
    public interface IJobService
    {
        Task<IEnumerable<JobDTO>> GetJobsAsync(JobStatus? status = null);
        Task<JobDTO> GetJobByIdAsync(int id);
        Task<JobDTO> CreateJobAsync(JobDTO jobDto);
        Task<JobDTO> UpdateJobAsync(int id, JobDTO jobDto);
        Task<bool> DeleteJobAsync(int id);
        Task<IEnumerable<EmployeeDTO>> GetJobEmployeesAsync(int jobId);
        Task<bool> AssignEmployeeToJobAsync(int jobId, int employeeId);
        Task<bool> RemoveEmployeeFromJobAsync(int jobId, int employeeId);
        Task<bool> UpdateJobStatusAsync(int id, JobStatus status);
    }
}
