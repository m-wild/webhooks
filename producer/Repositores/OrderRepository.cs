using System;
using Dapper;
using producer.Entities;

namespace producer.Repositories
{
    public interface IOrderRepository
    {
        Order GetById(int orderId);

        void Create(Order order);

        void Update(Order order);
    }
    
    public class OrderRepository : IOrderRepository
    {
        private readonly IDatabase db;

        public OrderRepository(IDatabase db)
        {
            this.db = db;
        }

        public Order GetById(int orderId)
        {
            var order = db.Connection.QuerySingleOrDefault<Order>(
                "SELECT * FROM orders WHERE order_id = @OrderId;",
                new { orderId });

            return order ?? throw new Exception("Order not found");
        }

        public void Create(Order order)
        {
            order.OrderId = db.Connection.QuerySingleOrDefault<int>(
                "INSERT INTO orders (product_code) VALUES (@ProductCode); " + 
                "SELECT LAST_INSERT_ID();",
                new { order.ProductCode });
        }

        public void Update(Order order)
        {
            db.Connection.Execute(
                "REPLACE INTO orders (order_id, product_code, created_at, processed_at) VALUES (@OrderId, @ProductCode, @CreatedAt, @ProcessedAt);",
                order);
        }
    }
}