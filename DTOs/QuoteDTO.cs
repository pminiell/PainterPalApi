using PainterPalApi.Models;

namespace PainterPalApi.DTOs
{
    public class QuoteDTO
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int CustomerId { get; set; }
        public ICollection<string> Tags { get; set; }
        public ICollection<string> Tasks { get; set; }
        public ICollection<ColourDTO> PrefferredColours { get; set; }
        public QuoteStatus QuoteStatus { get; set; }
        public string QuoteNotes { get; set; }
        public string QuotePrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CustomerDTO Customer { get; set; }
    }
}
