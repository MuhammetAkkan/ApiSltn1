using ApiCourseSltn2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCourseSltn2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private static List<Product>? _products;
        
        public ProductController()
        {
            _products = new List<Product>
            {
                new() {ProductId = 1, ProductName = "Iphone 13", IsActive = true, Price = 130},
                new() {ProductId = 2, ProductName = "Iphone 14", IsActive = false, Price = 140},
                new() {ProductId = 3, ProductName = "Iphone 15", IsActive = true, Price = 150},
            };
        }

        [HttpGet("GetProducts")]
        public IActionResult GetProducts()
        {
            var result = _products;
            
            if (result is null)
                return NotFound(); //null olma durumunda 404 dönüyoruz.
            
            return Ok(result);
        }

        [HttpPost("GetProduct{id}")]
        public IActionResult GetProduct(int? id)
        {
            if (id is null)
                return NotFound();

            var result  = _products?.FirstOrDefault(p=> p.ProductId == id);
            
            if(result is null)
                return NotFound();

            return Ok(result);
        }
    }
}
