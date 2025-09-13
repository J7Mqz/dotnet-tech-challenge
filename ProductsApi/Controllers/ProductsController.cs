using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class ProductsController : ControllerBase
    {
        // Para este reto, una lista en memoria es suficiente para empezar.
        private static readonly List<Product> _products = new List<Product>
        {
            new Product { ExternalId = "p-1001", Name = "Mouse", Price = 15.99m, Currency = "USD", UpdatedAtUtc = DateTime.UtcNow.AddDays(-1) },
            new Product { ExternalId = "p-1002", Name = "Teclado Mecánico", Price = 79.50m, Currency = "USD", UpdatedAtUtc = DateTime.UtcNow.AddHours(-5) },
            new Product { ExternalId = "p-1003", Name = "Monitor 24 pulgadas", Price = 150.00m, Currency = "USD", UpdatedAtUtc = DateTime.UtcNow.AddMinutes(-30) },
            new Product { ExternalId = "p-1004", Name = "Webcam HD", Price = 45.99m, Currency = "USD", UpdatedAtUtc = DateTime.UtcNow.AddDays(-2) },
            new Product { ExternalId = "p-1005", Name = "Auriculares con Micrófono", Price = 33.25m, Currency = "USD", UpdatedAtUtc = DateTime.UtcNow }
        };

        [HttpGet]
        public IActionResult GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var totalItems = _products.Count;
            var items = _products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            
            var response = new
            {
                items = items,
                page = page,
                pageSize = pageSize,
                total = totalItems
            };

            return Ok(response);
        }
    }
}