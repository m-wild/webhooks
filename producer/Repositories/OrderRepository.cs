using System;
using Dapper;
using Producer.Entities;
using Producer.Repositories;

namespace Producer.Repositories
{
    public interface IOrderRepository
    {
        Order GetById(int orderId);

        void Create(Order order);

        void Update(Order order);
    }
    
    public class OrderRepository : IOrderRepository
    {
        private readonly IDatabase _db;

        public OrderRepository(IDatabase db)
        {
            _db = db;
        }

        public Order GetById(int orderId)
        {
            var order = _db.Connection.QuerySingleOrDefault<Order>(
                "SELECT * FROM orders WHERE order_id = @OrderId;",
                new { orderId },
                transaction: _db.Transaction);

            return order ?? throw new Exception("Order not found");
        }

        public void Create(Order order)
        {
            order.OrderId = _db.Connection.ExecuteScalar<int>(
                "INSERT INTO orders (product_code) VALUES (@ProductCode); " + 
                "SELECT LAST_INSERT_ID();",
                new { order.ProductCode },
                transaction: _db.Transaction);
        }

        public void Update(Order order)
        {
            _db.Connection.Execute(
                "REPLACE INTO orders (order_id, product_code, created_at, processed_at) VALUES (@OrderId, @ProductCode, @CreatedAt, @ProcessedAt);",
                order,
                transaction: _db.Transaction);
        }
    }
}