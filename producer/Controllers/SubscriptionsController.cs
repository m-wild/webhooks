using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Producer.Entities;
using Producer.Repositories;

namespace Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionsController(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }
        
        [HttpGet]
        public List<Subscription> GetAll()
        {
            return _subscriptionRepository.GetAll();
        }

        [HttpPost]
        public int CreateSubscription([FromBody] Subscription subscription)
        {
            _subscriptionRepository.Create(subscription);

            return subscription.SubscriptionId;
        }
    }
}
