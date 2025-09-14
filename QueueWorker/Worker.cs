using Azure.Messaging.ServiceBus;
using QueueWorker.Messages;
using QueueWorker.Services;
using System.Text.Json;

namespace QueueWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ServiceBusClient _serviceBusClient;
        private ServiceBusProcessor _processor;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            string serviceBusConnectionString = "Endpoint=sb://retotech.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mn5J2o2e85bxNN2IpgiwKc8eUWx5tyiG7+ASbFhPCWA=";
            _serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor = _serviceBusClient.CreateProcessor("products-queue", new ServiceBusProcessorOptions());
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            _logger.LogInformation("Worker iniciado. Escuchando la cola 'products-queue'.");
            await _processor.StartProcessingAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_processor != null)
            {
                _logger.LogInformation("Deteniendo el procesador de mensajes.");
                await _processor.StopProcessingAsync(cancellationToken);
            }
            await base.StopAsync(cancellationToken);
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            _logger.LogInformation("Procesando mensaje. MessageId: {MessageId}", args.Message.MessageId);

            try
            {
                var productMessage = JsonSerializer.Deserialize<ProductMessage>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (productMessage == null) throw new JsonException("El cuerpo del mensaje no se pudo deserializar.");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var processorService = scope.ServiceProvider.GetRequiredService<IMessageProcessorService>();
                    await processorService.ProcessProductMessageAsync(productMessage);
                }

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el mensaje con MessageId: {MessageId}", args.Message.MessageId);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Error en el procesador de Service Bus.");
            return Task.CompletedTask;
        }
    }
}