namespace ProductsApi.Models
{
    public class Product
    {
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}