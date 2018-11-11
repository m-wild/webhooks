using Producer.Entities;
using Producer.Repositories;

namespace Producer.Services
{
    public interface IEventService
    {
        void Add(Event evnt);
    }
    
    public class EventService : IEventService
    {
        private readonly IEventQueue _eventQueue;
        private readonly IEventRepository _eventRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public EventService(IEventQueue eventQueue, IEventRepository eventRepository, ISubscriptionRepository subscriptionRepository)
        {
            _eventQueue = eventQueue;
            _eventRepository = eventRepository;
            _subscriptionRepository = subscriptionRepository;
        }
                
        public void Add(Event evnt)
        {
            // save the event to the db
            _eventRepository.CreateEvent(evnt);
            
            // link the event to each sub
            var eventSubs = _subscriptionRepository.CreateEventSubscription(evnt);

            // queue all events for processing
            foreach (var es in eventSubs)
            {
                _eventQueue.Add(es);
            }
        }
    }
}