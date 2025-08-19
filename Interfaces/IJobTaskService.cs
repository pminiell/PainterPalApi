using PainterPalApi.DTOs;
using PainterPalApi.Models;

namespace PainterPalApi.Interfaces
{
    public interface IJobTaskService
    {
        Task<IEnumerable<JobTaskDTO>> GetJobTasksByJobIdAsync(int jobId);
        Task<JobTaskDTO> GetJobTaskByIdAsync(int id);
        Task<JobTaskDTO> CreateJobTaskAsync(JobTaskDTO jobTaskDto);
        Task<JobTaskDTO> UpdateJobTaskAsync(int id, JobTaskDTO jobTaskDto);
        Task<bool> DeleteJobTaskAsync(int id);
        Task<bool> CompleteJobTaskAsync(int id);
        Task<bool> UncompleteJobTaskAsync(int id);
    }
}
