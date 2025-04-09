namespace PainterPalApi.Models
{
    public class UserMaterialPreference
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MaterialId { get; set; }
        
        public bool IsFavourite { get; set; }
        public User User { get; set; }
        public Material Material { get; set; }
    }
}