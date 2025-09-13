var builder = WebApplication.CreateBuilder(args);

// 1. Añadir servicios para Controllers
builder.Services.AddControllers();

// 2. Añadir servicios para Swagger (sin configuración de seguridad)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 3. Mapear los controllers
app.MapControllers();

app.Run();