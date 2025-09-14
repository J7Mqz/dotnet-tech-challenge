// Services/IProductService.cs
using ProductsApi.Models;

namespace ProductsApi.Services
{
    public interface IProductService
    {
        Task<PagedResponse<Product>> GetProductsAsync(int page, int pageSize);
    }
}