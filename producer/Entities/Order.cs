using System;

namespace producer.Entities
{
    public class Order
    {
        public int OrderId { get; set; }

        public string ProductCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ProcessedAt { get; set; }
    }
}