using PainterPalApi.Models;

namespace PainterPalApi.DTOs
{
    public class JobDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string JobName { get; set; }
        public string JobNotes { get; set; }
        public string JobLocation { get; set; }
        public JobStatus JobStatus { get; set; }
        public ICollection<string> Tags { get; set; }
        public ICollection<JobTaskDTO> JobTaskList { get; set; }
        public ICollection<ColourDTO> PreferredColours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CustomerDTO Customer { get; set; }
    }
}
