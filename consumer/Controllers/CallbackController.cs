using System;
using consumer.Entities;
using consumer.Repositories;
using Consumer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace consumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IOrderRepository _orderRepository;

        public CallbackController(IEmailRepository emailRepository, IOrderRepository orderRepository)
        {
            _emailRepository = emailRepository;
            _orderRepository = orderRepository;
        }
        
        [HttpPost]
        public void Post([FromBody] JObject value)
        {
            // example of handling an event 
            
            var orderEvent = value.ToObject<OrderEvent>();
            
            var orderInfo = JsonConvert.DeserializeObject<OrderEventData>(orderEvent.Body);

            if (string.Equals(orderEvent.EventType, "OrderCreated", StringComparison.OrdinalIgnoreCase))
            {
                var email = new Email
                {
                    Body = $"New order created with OrderID {orderInfo.OrderId}!",
                };
                _emailRepository.Create(email);
            }
            
            _orderRepository.AcknowledgeEvent(orderEvent.EventId);
        }
    }
}
