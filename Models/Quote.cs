namespace PainterPalApi.Models
{
    public class Quote
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int CustomerId { get; set; }
        //unsure how this interaction will work
        public int ProductPreferenceId { get; set; }
        public int ColourId { get; set; }
        public QuoteStatus QuoteStatus { get; set; }
        public string QuoteDescription { get; set; }
        public string QuotePrice { get; set; } 
        public Job Job { get; set; }
        public Customer Customer { get; set; }
    }

    public enum QuoteStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }
}
