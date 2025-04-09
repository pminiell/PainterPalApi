namespace PainterPalApi.Models
{
    public class UserPreferredColour
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ColourId { get; set; }
        
        public bool IsFavourite { get; set; }
        public User User { get; set; }
        public Colour Colour { get; set; }
    }
}