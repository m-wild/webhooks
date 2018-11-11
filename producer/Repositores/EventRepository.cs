using System;
using Dapper;
using producer.Entities;

namespace producer.Repositories
{
    public interface IEventRepository
    {
        void CreateEvent(Event evnt);
    }
    
    public class EventRepository : IEventRepository
    {
        private readonly IDatabase db;

        public EventRepository(IDatabase db)
        {
            this.db = db;
        }
        
        public void CreateEvent(Event evnt)
        {
            db.Connection.Execute(
                "INSERT INTO events (event_type_id, body) VALUES (@EventType, @Body);",
                new {evnt.EventType, evnt.Body});
        }
    }
}