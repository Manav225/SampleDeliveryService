using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleDeliveryService.Models;

namespace SampleDeliveryService.Services
{
    public class FakeCosmosDbService : ICosmosDbService
    {
        private static readonly List<Order> _orders = new List<Order>
        {
            new Order { Id = "1", FirstName = "Alice", LastName = "Smith", Packages = 3, Street = "456 Cosmos Ln", City = "Seattle", State = "WA", ZipCode = 98101, Latitude = "47.6062", Longitude = "-122.3321", Completed = false },
            new Order { Id = "2", FirstName = "Bob", LastName = "Johnson", Packages = 1, Street = "789 Delivery Blvd", City = "Bellevue", State = "WA", ZipCode = 98004, Latitude = "47.6101", Longitude = "-122.2015", Completed = true },
            new Order { Id = "3", FirstName = "Aarav", LastName = "Deshmukh", Packages = 2, Street = "101 MG Road", City = "Pune", State = "Maharashtra", ZipCode = 411001, Latitude = "18.5204", Longitude = "73.8567", Completed = false },
            new Order { Id = "4", FirstName = "Neha", LastName = "Patil", Packages = 1, Street = "202 Bandra West", City = "Mumbai", State = "Maharashtra", ZipCode = 400050, Latitude = "19.0760", Longitude = "72.8777", Completed = true },
            new Order { Id = "5", FirstName = "Rahul", LastName = "Jadhav", Packages = 4, Street = "303 Civil Lines", City = "Nagpur", State = "Maharashtra", ZipCode = 440001, Latitude = "21.1458", Longitude = "79.0882", Completed = false },
            new Order { Id = "6", FirstName = "Sneha", LastName = "Kulkarni", Packages = 2, Street = "Shivaji Chowk", City = "Nashik", State = "Maharashtra", ZipCode = 422001, Latitude = "19.9975", Longitude = "73.7898", Completed = false },
            new Order { Id = "7", FirstName = "Vikas", LastName = "Bhosale", Packages = 1, Street = "MIDC Area", City = "Aurangabad", State = "Maharashtra", ZipCode = 431001, Latitude = "19.8762", Longitude = "75.3433", Completed = true },
            new Order { Id = "8", FirstName = "Priya", LastName = "Naik", Packages = 3, Street = "Main Market", City = "Kolhapur", State = "Maharashtra", ZipCode = 416001, Latitude = "16.7050", Longitude = "74.2433", Completed = false },
            new Order { Id = "9", FirstName = "Kiran", LastName = "Sawant", Packages = 2, Street = "Old Bus Stand", City = "Solapur", State = "Maharashtra", ZipCode = 413001, Latitude = "17.6599", Longitude = "75.9064", Completed = true },
            new Order { Id = "10", FirstName = "Sakshi", LastName = "Shinde", Packages = 5, Street = "Jijamata Nagar", City = "Thane", State = "Maharashtra", ZipCode = 400601, Latitude = "19.2183", Longitude = "72.9781", Completed = false },
            new Order { Id = "11", FirstName = "Aniket", LastName = "Rane", Packages = 2, Street = "Panvel Junction", City = "Navi Mumbai", State = "Maharashtra", ZipCode = 410206, Latitude = "18.9894", Longitude = "73.1175", Completed = false },
            new Order { Id = "12", FirstName = "Meera", LastName = "Bhagat", Packages = 1, Street = "Satara Road", City = "Satara", State = "Maharashtra", ZipCode = 415001, Latitude = "17.6805", Longitude = "74.0183", Completed = true }
        };

        public Task AddItemAsync(Order order)
        {
            return Task.CompletedTask;
        }

        public Task DeleteItemAsync(string id)
        {
            return Task.CompletedTask;
        }

        public Task<Order> GetItemAsync(string id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);

            // Return matching order or default
            return Task.FromResult(order ?? new Order
            {
                Id = id,
                FirstName = "Default",
                LastName = "Fallback",
                Packages = 1,
                Street = "Default Street",
                City = "Thane",
                State = "Maharashtra",
                ZipCode = 400601,
                Latitude = "19.2183",
                Longitude = "72.9781",
                Completed = false
            });
        }

        public Task<IEnumerable<Order>> GetItemsAsync(string query)
        {
            return Task.FromResult((IEnumerable<Order>)_orders);
        }

        public Task UpdateItemAsync(Order updatedOrder)
        {
            var existing = _orders.FirstOrDefault(o => o.Id == updatedOrder.Id);
            if (existing != null)
            {
                existing.FirstName = updatedOrder.FirstName;
                existing.LastName = updatedOrder.LastName;
                existing.Packages = updatedOrder.Packages;
                existing.Street = updatedOrder.Street;
                existing.City = updatedOrder.City;
                existing.State = updatedOrder.State;
                existing.ZipCode = updatedOrder.ZipCode;
                existing.Latitude = updatedOrder.Latitude;
                existing.Longitude = updatedOrder.Longitude;
                existing.Completed = updatedOrder.Completed;
            }
            return Task.CompletedTask;
        }

    }
}
