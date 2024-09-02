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

        [HttpGet("GetProduct{id}")]
        public async Task<IActionResult> GetProduct(int? id)
        {
            if (id is null)
                return NotFound();

            var result = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);
            
            if(result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProducts(Product? entity)
        {
            if(entity is null)
                return BadRequest("Geçersiz veri!");

            var product = await _context.Products.FirstOrDefaultAsync(i=> i.ProductName == entity.ProductName);
            if (product is not null)
                return BadRequest("Ürün zaten mevcut");

            try
            {
                _context.Products.Add(entity);
                await _context.SaveChangesAsync();
                
                return CreatedAtAction(nameof(GetProduct), new { id = entity.ProductId }, entity); //GetProduct api ye ürünü gönderiyoruz ve döndürüyoruz. Yine bu kısımda dönüyor ama bu kod ile eklemediğimiz entity i net bir şekilde api de görebiliyoruz. Ayrıca read e gitmemize gerek kalmıyor.
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Db ye kaydedilemedi{entity.ProductName} + {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id is null)
                return BadRequest("Geçersiz veri");

            var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);
            if(product is null)
                return BadRequest("Bu ürün kayıtlı değil");

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetProducts), product);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Db den {id} ye sahip ürün silinemedi.\n {ex.Message}");
            }

        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(int? id, Product updatedProduct)
        {
            if (id is null)
                return BadRequest("Geçersiz veri");

            var product = await _context.Products.FirstOrDefaultAsync(i=> i.ProductId == id);
            
            if (product is null)
                return BadRequest("Bu ürün kayıtlı değil");

            try
            {
                product.ProductId = updatedProduct.ProductId;
                product.ProductName = updatedProduct.ProductName;
                product.Price = updatedProduct.Price;
                product.IsActive = updatedProduct.IsActive;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProducts), product);
            }
            catch (Exception ex)
            {
                return StatusCode(statusCode:500, ex.Message);
            }
        }

    }
}
