using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProductsPublisherAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PublishController> _logger;

        public PublishController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<PublishController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("products")]
        public async Task<IActionResult> PublishProducts()
        {
            var correlationId = Guid.NewGuid();
            _logger.LogInformation("Inicio del proceso de publicación con CorrelationId: {CorrelationId}", correlationId);

            try
            {
                // 1. OBTENER TOKEN DE AUTHSERVER
                var authClient = _httpClientFactory.CreateClient("AuthServer");
                var loginRequest = new { username = "admin", password = "password" };
                var authResponse = await authClient.PostAsJsonAsync("/api/auth/login", loginRequest);
                authResponse.EnsureSuccessStatusCode();
                var authContent = await authResponse.Content.ReadFromJsonAsync<AuthResponse>();

                // 2. OBTENER PRODUCTOS DE PRODUCTSAPI
                var productsClient = _httpClientFactory.CreateClient("ProductsApi");
                productsClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", authContent.AccessToken);

                var productsResponse = await productsClient.GetFromJsonAsync<PagedProductsResponse>("/api/products?pageSize=100");

                if (productsResponse?.Items == null || !productsResponse.Items.Any())
                {
                    _logger.LogWarning("No se encontraron productos para publicar.");
                    return Ok("No se encontraron productos para publicar.");
                }

                // 3. PUBLICAR EN AZURE SERVICE BUS
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
                return Ok(new { message = $"{productsResponse.Items.Count} productos publicados.", correlationId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el proceso de publicación con CorrelationId: {CorrelationId}", correlationId);
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }
    }

    // Clases auxiliares para deserializar las respuestas
    public class AuthResponse {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } }
    public class PagedProductsResponse { public List<Product> Items { get; set; } }
    public class Product { public string ExternalId { get; set; } public string Name { get; set; } public decimal Price { get; set; } public string Currency { get; set; } }
}