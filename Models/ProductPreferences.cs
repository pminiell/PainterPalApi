namespace PainterPalApi.Models
{
    public class ProductPreferences 
    {
        public int Id { get; set; }
        public int PreferredColourId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductType { get; set; }
        public Colour Colour { get; set; }
        public string ProductFinish { get; set; }
        public string ProductSize { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
