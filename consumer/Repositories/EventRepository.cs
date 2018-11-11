using consumer.Entities;
using Consumer.Repositories;
using Dapper;

namespace consumer.Repositories
{
    public interface IEmailRepository
    {
        void Create(Email email);

    }

    public class EmailRepository : IEmailRepository
    {
        private readonly IDatabase _db;
        
        public EmailRepository(IDatabase db)
        {
            _db = db;
        }

        public void Create(Email email)
        {
            email.EmailId = _db.Connection.ExecuteScalar<int>(
                "INSERT INTO emails (body, created_at) VALUES (@Body, @CreatedAt); " +
                "SELECT LAST_INSERT_ID();",
                new {email.Body, email.CreatedAt},
                transaction: _db.Transaction);
        }
    }
}