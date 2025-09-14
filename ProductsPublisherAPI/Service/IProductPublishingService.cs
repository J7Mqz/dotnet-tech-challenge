namespace ProductsPublisherAPI.Services
{
    public interface IProductPublishingService
    {
        Task<PublishResult> PublishProductsAsync();
    }

    // Un pequeño "record" para devolver un resultado claro del servicio.
    public record PublishResult(bool IsSuccess, int ProductsPublishedCount, Guid CorrelationId, string? ErrorMessage);
}