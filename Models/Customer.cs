using System.ComponentModel.DataAnnotations;

namespace PainterPalApi.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; }

        [Required]
        [Phone]
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
