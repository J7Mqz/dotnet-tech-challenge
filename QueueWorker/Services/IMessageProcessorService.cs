using QueueWorker.Messages;

namespace QueueWorker.Services
{
    public interface IMessageProcessorService
    {
        Task ProcessProductMessageAsync(ProductMessage productMessage);
    }
}