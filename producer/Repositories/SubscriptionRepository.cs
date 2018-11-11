using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Producer.Entities;

namespace Producer.Repositories
{
    public interface ISubscriptionRepository
    {
        void Create(Subscription sub);

        List<Subscription> GetAll();

        Subscription GetById(int subscriptionId);

        List<EventSubscription> CreateEventSubscription(Event evnt);

        void UpdateEventSubscription(EventSubscription eventSubscription);

        List<EventSubscription> GetUnpublishedEventSubscriptions();
    }
    
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IDatabase _db;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "__SubscriptionRepository_Subscriptions";

        public SubscriptionRepository(IDatabase db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }
        
        public void Create(Subscription sub)
        {
            _cache.Remove(CacheKey);
            
            sub.SubscriptionId = _db.Connection.ExecuteScalar<int>(
                "INSERT INTO subscriptions (name, uri) VALUES (@Name, @Uri);" +
                "SELECT LAST_INSERT_ID();",
                new {sub.Name, sub.Uri},
                transaction: _db.Transaction);
        }

        public List<Subscription> GetAll()
        {
            return _cache.GetOrCreate(CacheKey, entry =>
            {
                return _db.Connection.Query<Subscription>(
                        "SELECT * FROM subscriptions;",
                        transaction: _db.Transaction)
                    .ToList();
            });
        }

        public Subscription GetById(int subscriptionId)
        {
            var subscriptions = GetAll();

            return subscriptions.SingleOrDefault(sub => sub.SubscriptionId == subscriptionId);
        }

        public List<EventSubscription> CreateEventSubscription(Event evnt)
        {
            var subscriptions = GetAll();

            var eventSubscriptions = subscriptions
                .Select(sub => new EventSubscription(sub, evnt))
                .ToList();

            _db.Connection.Execute(
                "INSERT INTO event_subscriptions (subscription_id, event_id, created_at, published_at) " +
                "VALUES (@SubscriptionId, @EventId, @CreatedAt, @PublishedAt);",
                eventSubscriptions,
                transaction: _db.Transaction);

            return eventSubscriptions;
        }

        public void UpdateEventSubscription(EventSubscription eventSubscription)
        {
            _db.Connection.Execute(
                "REPLACE INTO event_subscriptions (subscription_id, event_id, created_at, published_at) " +
                "VALUES (@SubscriptionId, @EventId, @CreatedAt, @PublishedAt);",
                eventSubscription,
                transaction: _db.Transaction);
        }

        public List<EventSubscription> GetUnpublishedEventSubscriptions()
        {
            return _db.Connection.Query<EventSubscription>(
                    "SELECT * FROM event_subscriptions WHERE published_at IS NULL;",
                    transaction: _db.Transaction)
                .ToList();
        }
    }
}