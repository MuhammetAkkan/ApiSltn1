using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCourseSltn2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private static readonly string[] ProductNames = new[]
        {
            "Laptop", "Tablet", "Smartphone", "Monitor", "Keyboard", "Mouse", "Headphones", "Printer", "Camera", "Speaker"
        };

        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        // Get all orders
        [HttpGet(Name = "GetOrders")]
        public IEnumerable<Order> GetOrders()
        {
            return Enumerable.Range(1, 1).Select(index => new Order
            {
                OrderId = index,
                ProductName = ProductNames[Random.Shared.Next(ProductNames.Length)],
                Quantity = Random.Shared.Next(1, 10),
                OrderDate = DateTime.Now.AddDays(-index),
                TotalPrice = Random.Shared.Next(100, 1000)
            })
            .ToArray();
        }

        // Get orders sorted by total price descending
        [HttpGet("sorted", Name = "GetSortedOrders")]
        public IEnumerable<Order> GetSortedOrders()
        {
            return Enumerable.Range(1, 2).Select(index => new Order
            {
                OrderId = index,
                ProductName = ProductNames[Random.Shared.Next(ProductNames.Length)],
                Quantity = Random.Shared.Next(1, 10),
                OrderDate = DateTime.Now.AddDays(-index),
                TotalPrice = Random.Shared.Next(100, 1000)
            })
            .OrderByDescending(order => order.TotalPrice)
            .ToArray();
        }

       

        // Get orders filtered by a minimum total price
        [HttpGet("filter", Name = "GetFilteredOrders")]
        public IEnumerable<Order> GetFilteredOrders([FromQuery] int minPrice)
        {
            return Enumerable.Range(1, 3).Select(index => new Order
            {
                OrderId = index,
                ProductName = ProductNames[Random.Shared.Next(ProductNames.Length)],
                Quantity = Random.Shared.Next(1, 10),
                OrderDate = DateTime.Now.AddDays(-index),
                TotalPrice = Random.Shared.Next(100, 1000)
            })
            .Where(order => order.TotalPrice >= minPrice)
            .ToArray();
        }
    }
}
