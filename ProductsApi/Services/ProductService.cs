// Services/ProductService.cs
using ProductsApi.Models;

namespace ProductsApi.Services
{
    public class ProductService : IProductService
    {
        private static readonly List<Product> _products = new List<Product>
        {
            new Product { ExternalId = "p-1001", Name = "Mouse", Price = 15.99m, Currency = "USD", UpdatedAtUtc = DateTime.UtcNow.AddDays(-1) },
            new Product { ExternalId = "p-1002", Name = "Teclado Mecánico", Price = 79.50m, Currency = "USD", UpdatedAtUtc = DateTime.UtcNow.AddHours(-5) },
            new Product { ExternalId = "p-1003", Name = "Monitor 24 pulgadas", Price = 150.00m, Currency = "USD", UpdatedAtUtc = DateTime.UtcNow.AddMinutes(-30) },
            new Product { ExternalId = "p-1004", Name = "Webcam HD", Price = 45.99m, Currency = "USD", UpdatedAtUtc = DateTime.UtcNow.AddDays(-2) },
            new Product { ExternalId = "p-1005", Name = "Auriculares con Micrófono", Price = 33.25m, Currency = "USD", UpdatedAtUtc = DateTime.UtcNow }
        };

        public Task<PagedResponse<Product>> GetProductsAsync(int page, int pageSize)
        {
            var totalItems = _products.Count;
            var items = _products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var response = new PagedResponse<Product>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                Total = totalItems
            };

            return Task.FromResult(response);
        }
    }
}