using System.Collections.Generic;
using System.Threading.Tasks;
using SampleDeliveryService.Models;

namespace SampleDeliveryService.Services
{
    public class FakeCosmosDbService : ICosmosDbService
    {
        public Task AddItemAsync(Order order)
        {
            // Simulate adding an item (do nothing)
            return Task.CompletedTask;
        }

        public Task DeleteItemAsync(string id)
        {
            // Simulate delete (do nothing)
            return Task.CompletedTask;
        }

        public Task<Order> GetItemAsync(string id)
        {
            // Return a dummy order with realistic properties
            var order = new Order
            {
                Id = id,
                FirstName = "Test",
                LastName = "User",
                Packages = 2,
                Street = "123 Azure Ave",
                City = "Redmond",
                State = "WA",
                ZipCode = 98052,
                Latitude = "47.673988",
                Longitude = "-122.121513",
                Completed = false
            };

            return Task.FromResult(order);
        }

        public Task<IEnumerable<Order>> GetItemsAsync(string query)
        {
            // Return a dummy list of orders
            var orders = new List<Order>
            {
                new Order
                {
                    Id = "1",
                    FirstName = "Alice",
                    LastName = "Smith",
                    Packages = 3,
                    Street = "456 Cosmos Ln",
                    City = "Seattle",
                    State = "WA",
                    ZipCode = 98101,
                    Latitude = "47.6062",
                    Longitude = "-122.3321",
                    Completed = false
                },
                new Order
                {
                    Id = "2",
                    FirstName = "Bob",
                    LastName = "Johnson",
                    Packages = 1,
                    Street = "789 Delivery Blvd",
                    City = "Bellevue",
                    State = "WA",
                    ZipCode = 98004,
                    Latitude = "47.6101",
                    Longitude = "-122.2015",
                    Completed = true
                }
            };

            return Task.FromResult((IEnumerable<Order>)orders);
        }

        public Task UpdateItemAsync(Order order)
        {
            // Simulate update (do nothing)
            return Task.CompletedTask;
        }
    }
}
