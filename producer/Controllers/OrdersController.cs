using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using producer.Entities;
using producer.Repositories;

namespace producer.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController
    {
        private readonly IOrderRepository orderRepository;
        private readonly IEventRepository eventRepository;

        public OrdersController(IOrderRepository orderRepository, IEventRepository eventRepository)
        {
            this.orderRepository = orderRepository;
            this.eventRepository = eventRepository;
        }

        [HttpGet]
        [Route("{orderId}")]
        public Order GetOrder([FromRoute] int orderId)
        {
            return orderRepository.GetById(orderId);
        }

        [HttpPost]
        public int CreateOrder([FromBody] Order order)
        {
            orderRepository.Create(order);
            
            var e = new Event(EventType.OrderCreated, new { order.OrderId });
            eventRepository.CreateEvent(e);
            
            return order.OrderId;
        }

        [HttpPost]
        [Route("{orderId}")]
        public void ProcessOrder([FromRoute] int orderId)
        {
            var order = orderRepository.GetById(orderId);
            
            Thread.Sleep(100); // "process" the order...
            order.ProcessedAt = DateTime.Now;
            
            orderRepository.Update(order);
            
            var e = new Event(EventType.OrderProcessed, new { order.OrderId });
            eventRepository.CreateEvent(e);
        }

    }



}
