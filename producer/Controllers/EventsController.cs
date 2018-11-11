using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Producer.Entities;
using Producer.Repositories;

namespace Producer.Controllers
{
    [Route("api/[controller]")]
    public class EventsController
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }


        [HttpGet]
        public List<Event> GetUnacknowledged()
        {
            return _eventRepository.GetUnacknowledged();
        }
        
    }
}