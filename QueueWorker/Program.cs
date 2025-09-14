using Microsoft.EntityFrameworkCore;
using QueueWorker;
using QueueWorker.Data;
using QueueWorker.Repositories;
using QueueWorker.Services;

var builder = Host.CreateApplicationBuilder(args);

// 1. Registrar el DbContext
builder.Services.AddDbContext<ProductsDbContext>(options =>
{
    // Reemplaza la línea de configuración con tu cadena directa
    string dbConnectionString = "Server=localhost,1433;Database=ProductsDB;User=sa;Password=TechChallenge_123!;TrustServerCertificate=True;";
    options.UseSqlServer(dbConnectionString);
});


// 2. Registrar el Repositorio y el Servicio
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IMessageProcessorService, MessageProcessorService>();

// 3. Registrar el Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();