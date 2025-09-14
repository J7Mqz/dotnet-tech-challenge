using Azure.Messaging.ServiceBus;
using ProductsPublisherAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ProductsPublisherAPI.Services
{
    public class ProductPublishingService : IProductPublishingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductPublishingService> _logger;

        public ProductPublishingService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ProductPublishingService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<PublishResult> PublishProductsAsync()
        {
            var correlationId = Guid.NewGuid();
            _logger.LogInformation("Inicio del proceso de publicación con CorrelationId: {CorrelationId}", correlationId);

            try
            {
                // 1. OBTENER TOKEN
                var authClient = _httpClientFactory.CreateClient("AuthServer");
                var loginRequest = new { username = "admin", password = "password" };
                var authResponse = await authClient.PostAsJsonAsync("/api/auth/login", loginRequest);
                authResponse.EnsureSuccessStatusCode();
                var authContent = await authResponse.Content.ReadFromJsonAsync<AuthResponse>();

                if (authContent?.AccessToken is null)
                {
                    throw new InvalidOperationException("El token de acceso no se pudo obtener.");
                }

                // 2. OBTENER PRODUCTOS
                var productsClient = _httpClientFactory.CreateClient("ProductsApi");
                productsClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authContent.AccessToken);
                var productsResponse = await productsClient.GetFromJsonAsync<PagedProductsResponse>("/api/products?pageSize=100");

                if (productsResponse?.Items == null || !productsResponse.Items.Any())
                {
                    _logger.LogWarning("No se encontraron productos para publicar.");
                    return new PublishResult(true, 0, correlationId, "No se encontraron productos.");
                }

                // 3. PUBLICAR EN SERVICE BUS
                var connectionString = _configuration.GetConnectionString("ServiceBus");
                var queueName = "products-queue";
                await using var serviceBusClient = new ServiceBusClient(connectionString);
                ServiceBusSender sender = serviceBusClient.CreateSender(queueName);

                foreach (var product in productsResponse.Items)
                {
                    var messagePayload = new
                    {
                        product.ExternalId,
                        product.Name,
                        product.Price,
                        product.Currency,
                        FetchedAtUtc = DateTime.UtcNow,
                        CorrelationId = correlationId,
                        Source = "ProductsApi:/api/products"
                    };

                    var messageBody = JsonSerializer.Serialize(messagePayload);
                    var serviceBusMessage = new ServiceBusMessage(messageBody)
                    {
                        MessageId = $"product-{product.ExternalId}",
                        CorrelationId = correlationId.ToString()
                    };

                    await sender.SendMessageAsync(serviceBusMessage);
                }

                _logger.LogInformation("Se publicaron {ProductCount} productos con CorrelationId: {CorrelationId}", productsResponse.Items.Count, correlationId);
                return new PublishResult(true, productsResponse.Items.Count, correlationId, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el proceso de publicación con CorrelationId: {CorrelationId}", correlationId);
                return new PublishResult(false, 0, correlationId, ex.Message);
            }
        }
    }
}