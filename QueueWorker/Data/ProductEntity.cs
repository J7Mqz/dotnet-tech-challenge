using System.ComponentModel.DataAnnotations;

namespace QueueWorker.Data
{
    public class ProductEntity
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string ExternalId { get; set; } 

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public DateTime LastFetchedAtUtc { get; set; }
    }
}
