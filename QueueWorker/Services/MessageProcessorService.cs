using QueueWorker.Data;
using QueueWorker.Messages;
using QueueWorker.Repositories;

namespace QueueWorker.Services
{
    public class MessageProcessorService : IMessageProcessorService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<MessageProcessorService> _logger;

        public MessageProcessorService(IProductRepository productRepository, ILogger<MessageProcessorService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task ProcessProductMessageAsync(ProductMessage productMessage)
        {
            var existingProduct = await _productRepository.GetByExternalIdAsync(productMessage.ExternalId);

            if (existingProduct != null)
            {
                existingProduct.Name = productMessage.Name;
                existingProduct.Price = productMessage.Price;
                existingProduct.Currency = productMessage.Currency;
                existingProduct.LastFetchedAtUtc = productMessage.FetchedAtUtc;
                await _productRepository.UpdateAsync(existingProduct);
                _logger.LogInformation("Producto actualizado en la BD: {ExternalId}", productMessage.ExternalId);
            }
            else
            {
                var newProduct = new ProductEntity
                {
                    ExternalId = productMessage.ExternalId,
                    Name = productMessage.Name,
                    Price = productMessage.Price,
                    Currency = productMessage.Currency,
                    LastFetchedAtUtc = productMessage.FetchedAtUtc
                };
                await _productRepository.AddAsync(newProduct);
                _logger.LogInformation("Nuevo producto insertado en la BD: {ExternalId}", productMessage.ExternalId);
            }
        }
    }
}