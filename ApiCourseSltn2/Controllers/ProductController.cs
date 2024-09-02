using ApiCourseSltn2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCourseSltn2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductsContext _context;
        
        public ProductController(ProductsContext context)
        {
            _context = context;
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var result = await _context.Products.ToListAsync();
            
            if (result is null)
                return NotFound(); //null olma durumunda 404 dönüyoruz.
            
            return Ok(result);
        }

        [HttpPost("GetProduct{id}")]
        public async Task<IActionResult> GetProduct(int? id)
        {
            if (id is null)
                return NotFound();

            var result = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);
            
            if(result is null)
                return NotFound();

            return Ok(result);
        }
    }
}
