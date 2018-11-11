using System.Collections.Generic;
using System.Linq;
using Dapper;
using Producer.Entities;

namespace Producer.Repositories
{
    public interface IEventRepository
    {
        void CreateEvent(Event evnt);

        void UpdateEvent(Event evnt);
        
        List<Event> GetUnacknowledged();
    }
    
    public class EventRepository : IEventRepository
    {
        private readonly IDatabase _db;

        public EventRepository(IDatabase db)
        {
            _db = db;
        }
        
        public void CreateEvent(Event evnt)
        {
            evnt.EventId = _db.Connection.ExecuteScalar<int>(
                "INSERT INTO events (event_type_id, body) VALUES (@EventType, @Body);" +
                "SELECT LAST_INSERT_ID();",
                new {evnt.EventType, evnt.Body});
        }

        public void UpdateEvent(Event evnt)
        {
            _db.Connection.Execute(
                "REPLACE INTO events (event_id, event_type_id, body, published_at, acknowledged_at) " +
                "VALUES (@EventId, @EventType, @Body, @PublishedAt, @AcknowledgedAt);", 
                evnt);
        }

        public List<Event> GetUnacknowledged()
        {
            return _db.Connection.Query<Event>(
                    "SELECT event_id, event_type_id as event_type, body, published_at, acknowledged_at " +
                    "FROM events WHERE acknowledged_at IS NULL;")
                .ToList();

        }
    }
}