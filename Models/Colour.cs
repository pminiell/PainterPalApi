using System.ComponentModel.DataAnnotations;

namespace PainterPalApi.Models
{
    public class Colour
    {
        public int Id { get; set; }
        [Required]
        public string ColourName { get; set; }
        [Required]
        public string ColourCode { get; set; }
        [Required]
        public string ColourUsage { get; set; }
    }
}
