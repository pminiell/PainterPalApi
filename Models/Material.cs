namespace PainterPalApi.Models
{
    public class Material
    {
        public int Id { get; set; }
        public string MaterialName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal UnitPrice { get; set; }
        public int QuantityInStock { get; set; }
        public string Supplier { get; set; }
    }
}