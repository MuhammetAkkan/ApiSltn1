using ApiCourseSltn2.DTO;
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
            //1.yöntem
            //Where ifadesi sonrasında olur ise isActive e dair sorguyu yapamıyoruz ama ilk geldiğinde tüm productlar gelip dto ya basmadığımızdan isActive için olan filtrelemeyi yapabiliyoruz.

            /*
            var result = await _context.Products.Where(i=> i.IsActive == true).Select(item=> new ProductDTO  //burada i bize gelen nesneyi temsil etmekte onu başka bir nesne üzerine(DTO) ya basıyoruz ama sadece gerekli alanları
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price
            }).ToListAsync(); //data transfer object kullanarak orjinal product ı istediğimiz product versiyonunda sunduk.
            */
            

            //2.yöntem ki en kullanışlısı bu

            var result = await _context.Products.Where(i => i.IsActive == true).Select(product =>ProductToDTO(product)).ToListAsync();

            return result.Any() ? Ok(result) : NotFound(); //result var mı sorgusundan sonra duruma göre uygun olan gerçekleşecek.
            //True ise Ok(resut)
            //False ise Notfound()
        }


        [HttpGet("GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(int? id)
        {
            if (id is null)
                return BadRequest();

            var result = await _context.Products.FindAsync(id);
            
            return result is not null ? Ok(result) : NotFound();
        }


        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProducts(Product? entity)
        {
            if(entity is null)
                return BadRequest("Geçersiz veri!");

            var product = await _context.Products.AnyAsync(i => i.ProductName == entity.ProductName); //Var mı? durumunu sorgularken any daha mantıklıdır. FirstOrDefault daha çok hepsinin içinden tara ve getir şeklinde işe yarar.
            
            if (product is true)
                return NotFound("Ürün zaten mevcut");

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


        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int? id, Product updatedProduct)
        {
            if (id is null || id != updatedProduct.ProductId)
                return BadRequest("Geçersiz veri");

            var product = await _context.Products.FindAsync(id); //arasından arama ve getirme de ise FindAsync daha mantıklıdır.

            if (product is null)
                return NotFound("Bu ürün kayıtlı değil");

            try
            {
                product.ProductName = updatedProduct.ProductName; 
                product.Price = updatedProduct.Price;
                product.IsActive = updatedProduct.IsActive;

                //_context.Products.Update(product);
                await _context.SaveChangesAsync();
                return NoContent(); //eğer ki create değil ise NoContent yapıyoruz.
            }
            catch (Exception ex)
            {
                return StatusCode(statusCode: 500, ex.Message);
            }
        }


        [HttpDelete("DeleteProduct{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            
            var product = await _context.Products.FindAsync(id);
            
            if(product is null)
                return NotFound("Bu ürün kayıtlı değil"); //kullanıcı doğru veri yanlış => NotFound();

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Db den {id} ye sahip ürün silinemedi.\n {ex.Message}");
            }

        }   


        //sadece bu classta çalışan, product ı ProductDTO ya basan bir metot tanımladık.
        private static ProductDTO ProductToDTO(Product product)
        {
            return new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
            };
        }
        

    }
}
