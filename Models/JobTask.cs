// /home/pminiell/Documents/Coding/Dotnet/PainterPal/PainterPalApi/Models/Task.cs
namespace PainterPalApi.Models
{
    public class JobTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public JobTaskPriority Priority { get; set; } = JobTaskPriority.Medium;
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        
        // Navigation properties
        public int JobId { get; set; }
        public Job Job { get; set; }
    }
    
    public enum JobTaskPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Urgent = 3
    }
}