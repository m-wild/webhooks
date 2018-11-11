using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Producer.Entities;
using Producer.Infrastructure;
using Producer.Repositories;
using Producer.Services;

namespace Producer.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventService _eventService;

        public OrdersController(IOrderRepository orderRepository, IEventService eventService)
        {
            _orderRepository = orderRepository;
            _eventService = eventService;
        }

        [HttpGet]
        [Route("{orderId}")]
        public Order GetOrder([FromRoute] int orderId)
        {
            return _orderRepository.GetById(orderId);
        }

        [HttpPost]
        public int CreateOrder([FromBody] Order order)
        {
            _orderRepository.Create(order);
            
            var e = new Event(EventType.OrderCreated, new { order.OrderId });
            _eventService.Add(e);
            
            
            return order.OrderId;
        }

        [HttpPost]
        [Route("{orderId}")]
        public void ProcessOrder([FromRoute] int orderId)
        {
            var order = _orderRepository.GetById(orderId);
            
            Thread.Sleep(100); // "process" the order...
            order.ProcessedAt = DateTime.Now;
            
            _orderRepository.Update(order);
            
            var e = new Event(EventType.OrderProcessed, new { order.OrderId });
            _eventService.Add(e);
        }

    }



}
