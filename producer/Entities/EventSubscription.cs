using System;

namespace Producer.Entities
{
    public class EventSubscription
    {
        public EventSubscription()
        {
        }

        public EventSubscription(Subscription sub, Event evnt)
        {
            SubscriptionId = sub.SubscriptionId;
            EventId = evnt.EventId;
        }
        
        public int SubscriptionId { get; set; }

        public int EventId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? PublishedAt { get; set; }
    }
}