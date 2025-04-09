// /home/pminiell/Documents/Coding/Dotnet/PainterPal/PainterPalApi/DTOs/JobTaskDtos.cs
using PainterPalApi.Models;

namespace PainterPalApi.DTOs
{
    public class JobTaskDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public JobTaskPriority Priority { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int JobId { get; set; }
    }
    
    public class CreateJobTaskDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public JobTaskPriority Priority { get; set; } = JobTaskPriority.Medium;
    }
    
    public class UpdateJobTaskDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public JobTaskPriority Priority { get; set; }
        public bool IsCompleted { get; set; }
    }
}