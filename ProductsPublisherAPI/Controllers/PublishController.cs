using Microsoft.AspNetCore.Mvc;
using ProductsPublisherAPI.Services; 

namespace ProductsPublisherAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishController : ControllerBase
    {
        private readonly IProductPublishingService _publishingService;

        public PublishController(IProductPublishingService publishingService)
        {
            _publishingService = publishingService;
        }

        [HttpPost("products")]
        public async Task<IActionResult> PublishProducts()
        {
            var result = await _publishingService.PublishProductsAsync();

            if (result.IsSuccess)
            {
                return Ok(new { message = $"{result.ProductsPublishedCount} productos publicados.", correlationId = result.CorrelationId });
            }

            return StatusCode(500, new { message = "Ocurrió un error interno.", error = result.ErrorMessage, correlationId = result.CorrelationId });
        }
    }
}