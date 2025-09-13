var builder = WebApplication.CreateBuilder(args);

// 1. A�adir servicios para Controllers
builder.Services.AddControllers();

// 2. A�adir servicios para Swagger (sin configuraci�n de seguridad)
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