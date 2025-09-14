using Microsoft.EntityFrameworkCore;
using QueueWorker.Data;

namespace QueueWorker.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductsDbContext _dbContext;

        public ProductRepository(ProductsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductEntity?> GetByExternalIdAsync(string externalId)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(p => p.ExternalId == externalId);
        }

        public async Task AddAsync(ProductEntity product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductEntity product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }
    }
}