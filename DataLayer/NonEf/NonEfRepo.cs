using Dapper;
using DataLayer.EfClasses;
using GenericBizRunner;
using Microsoft.Data.Sqlite;
using System;

namespace DataLayer.NonEf
{
    public class NonEfRepo : IRepository
    {
        private readonly SqliteConnection _connection;
        public NonEfRepo(SqliteConnection connection)
        {
            _connection = connection;
        }

        public Order GetOrder(int OrderId)
        {
            Order order = _connection.QueryFirstOrDefault<Order>(
                "SELECT * FROM Orders WHERE OrderId = @OrderId;",
                new { OrderId = OrderId }
                );
            return order;
        }

        public bool ChangeOrderDeliveryDate(int OrderId, DateTime newDeliveryDate)
        {
            int affectedRows = _connection.Execute(
                "UPDATE Orders SET ExpectedDeliveryDate = @newDeliveryDate WHERE OrderId = @OrderId;",
                new { OrderId = OrderId, newDeliveryDate = newDeliveryDate }
                );
            return affectedRows == 1;
        }
    }
}
