namespace ProductsPublisherAPI.Models
{
    public class PagedProductsResponse
    {
        public List<Product> Items { get; set; } = new List<Product>();
    }
}
