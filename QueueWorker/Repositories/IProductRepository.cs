using QueueWorker.Data;

namespace QueueWorker.Repositories
{
    public interface IProductRepository
    {
        Task<ProductEntity?> GetByExternalIdAsync(string externalId);
        Task AddAsync(ProductEntity product);
        Task UpdateAsync(ProductEntity product);
    }
}