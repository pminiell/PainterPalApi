namespace PainterPalApi.Models
{
    public class Job
    {
        public int Id { get; set; }

        public Customer Customer { get; set; }

        public int ProductPreferenceId { get; set; }

        public ProductPreferences PreferredProduct { get; set; }

        public int CustomerId { get; set; }
        public string JobName { get; set; }
        public string JobDescription { get; set; }
        public string JobLocation { get; set; }
        public JobStatus JobStatus { get; set; }

        public ICollection<JobEmployee> EmployeeAssignments { get; set; } = new List<JobEmployee>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public DateTime? CompletionDate { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public enum JobStatus {
        Pending = 0,
        InProgress = 1,
        OnHold = 2,
        Completed = 3,
        Cancelled = 4
    }
}
