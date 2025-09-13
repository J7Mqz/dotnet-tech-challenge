var builder = WebApplication.CreateBuilder(args);

// --- INICIO DE CONFIGURACIÓN ---

// 1. Configurar IHttpClientFactory con clientes nombrados
builder.Services.AddHttpClient("AuthServer", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiClients:AuthServerUrl"]);
});

builder.Services.AddHttpClient("ProductsApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiClients:ProductsApiUrl"]);
});


// 2. Añadir servicios para Controllers y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- FIN DE CONFIGURACIÓN ---

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();