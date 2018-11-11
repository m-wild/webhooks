namespace consumer.Entities
{
    public class OrderEvent
    {
        public int EventId { get; set; }

        public string EventType { get; set; }

        public string Body { get; set; }
    }


    public class OrderEventData
    {
        public int OrderId { get; set; }
    }
    
}