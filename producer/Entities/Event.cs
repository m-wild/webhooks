using System;
using Newtonsoft.Json;

namespace producer.Entities
{
    public class Event
    {
        public Event()
        {
        }

        public Event(EventType eventType, object body)
        {
            EventType = eventType;
            Body = JsonConvert.SerializeObject(body);
        }


        public int EventId { get; set; }

        public EventType EventType { get; set; }    

        public string Body { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? AcknowledgedAt { get; set; }
    }

    public enum EventType
    {
        OrderCreated = 1,
        OrderProcessed = 2
    }
}