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

        Event GetById(int eventId);
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
                new {evnt.EventType, evnt.Body},
                transaction: _db.Transaction);
        }

        public void UpdateEvent(Event evnt)
        {
            _db.Connection.Execute(
                "UPDATE events " +
                "SET event_type_id = @EventType, body = @Body, acknowledged_at = @AcknowledgedAt " +
                "WHERE event_id = @EventId;", 
                evnt,
                transaction: _db.Transaction);
        }

        public List<Event> GetUnacknowledged()
        {
            return _db.Connection.Query<Event>(
                "SELECT event_id, event_type_id as event_type, body, acknowledged_at " +
                "FROM events WHERE acknowledged_at IS NULL;",
                transaction: _db.Transaction)
                .ToList();
        }

        public Event GetById(int eventId)
        {
            return _db.Connection.QuerySingleOrDefault<Event>(
                "SELECT event_id, event_type_id as event_type, body, acknowledged_at " +
                "FROM events WHERE event_id = @EventId;",
                new {eventId},
                transaction: _db.Transaction);
        }

    }
}