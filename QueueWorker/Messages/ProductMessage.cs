namespace QueueWorker.Messages
{
    public class ProductMessage
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime FetchedAtUtc { get; set; }
    }
}
