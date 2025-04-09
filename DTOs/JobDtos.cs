// PainterPalApi/DTOs/JobDTOs.cs
using PainterPalApi.Models;

namespace PainterPalApi.DTOs
{
    public class CreateJobDTO
    {
        public int CustomerId { get; set; }
        public string JobName { get; set; }
        public string JobNotes { get; set; }
        public string JobLocation { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<string> Tasks { get; set; } = new List<string>();
        public List<int> PreferredColourIds { get; set; } = new List<int>();
    }
    
    // Add other DTOs as needed
    public class UpdateJobDTO
    {
        public string JobName { get; set; }
        public string JobNotes { get; set; }
        public string JobLocation { get; set; }
        // Other updatable properties
    }

    public class JobDetailDTO
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public string JobNotes { get; set; }
        public string JobLocation { get; set; }
        public JobStatus JobStatus { get; set; }
        public ICollection<string> Tags { get; set; } = new List<string>();
        public ICollection<string> Tasks { get; set; } = new List<string>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? CompletionDate { get; set; }
        
        // Instead of the full Customer entity, just include essential customer details
        public CustomerSummaryDTO Customer { get; set; }
        
        // For colors, include their essential details
        public ICollection<ColourSummaryDTO> PreferredColours { get; set; } = new List<ColourSummaryDTO>();
    }
    
    public class JobSummaryDTO
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public string JobLocation { get; set; }
        public JobStatus JobStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
    
    public class CustomerSummaryDTO
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
    }
    
    public class ColourSummaryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Code { get; set; }
        public string Usage { get; set; }
    }
}