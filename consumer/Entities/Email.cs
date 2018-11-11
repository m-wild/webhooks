using System;

namespace consumer.Entities
{
    public class Email
    {
        public int EmailId { get; set; }

        public string Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? SentAt { get; set; }
    }
}