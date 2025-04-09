using System.Collections.Generic;
using System.Threading.Tasks;
using SampleDeliveryService.Models;

namespace SampleDeliveryService.Services
{
    public class FakeCosmosDbService : ICosmosDbService
    {
        public Task AddItemAsync(Order order)
        {
            // Do nothing
            return Task.CompletedTask;
        }

        public Task DeleteItemAsync(string id)
        {
            // Do nothing
            return Task.CompletedTask;
        }

        public Task<Order> GetItemAsync(string id)
        {
            // Return a dummy order
            return Task.FromResult(new Order
            {
                Id = id,
                CustomerName = "Test Customer",
                Product = "Test Product",
                Quantity = 1
            });
        }

        public Task<IEnumerable<Order>> GetItemsAsync(string query)
        {
            // Return a dummy list of orders
            var items = new List<Order>
            {
                new Order { Id = "1", CustomerName = "Alice", Product = "Item A", Quantity = 2 },
                new Order { Id = "2", CustomerName = "Bob", Product = "Item B", Quantity = 3 }
            };
            return Task.FromResult((IEnumerable<Order>)items);
        }

        public Task UpdateItemAsync(Order order)
        {
            // Do nothing
            return Task.CompletedTask;
        }
    }
}
