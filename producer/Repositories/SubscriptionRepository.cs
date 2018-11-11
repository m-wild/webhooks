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
                new {sub.Name, sub.Uri});
        }

        public List<Subscription> GetAll()
        {
            return _cache.GetOrCreate(CacheKey, entry => 
                _db.Connection.Query<Subscription>("SELECT * FROM subscriptions;").ToList());
        }
    }
}