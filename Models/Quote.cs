using System.ComponentModel.DataAnnotations;

namespace PainterPalApi.Models
{
    public class Quote
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int CustomerId { get; set; }

        public ICollection<string> Tags { get; set; } = new List<string>();
        public ICollection<string> Tasks { get; set; } = new List<string>();

        public ICollection<Colour> PrefferredColours { get; set; } = new List<Colour>();

        public QuoteStatus QuoteStatus { get; set; }

        public string QuoteNotes { get; set; } = "";

        public string QuotePrice { get; set; } = "0.00";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Customer Customer { get; set; }
    }

    public enum QuoteStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }
}
