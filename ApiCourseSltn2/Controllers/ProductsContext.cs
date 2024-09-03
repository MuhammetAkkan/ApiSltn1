using ApiCourseSltn2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiCourseSltn2.Controllers
{
    public class ProductsContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public DbSet<Product> Products { get; set; }
        public ProductsContext(DbContextOptions<ProductsContext> options) : base(options) //DbContextOption bir adet TContext almalı
        {
            
        }
        //@override + ctrl. => oradan zaten modelbuilder yazınca sadece bu geliyor.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(new Product { ProductId = 1, ProductName = "Iphone 11", Price = 1100, IsActive = true});
            modelBuilder.Entity<Product>().HasData(new Product { ProductId = 2, ProductName = "Iphone 12", Price = 1200, IsActive = false });
            modelBuilder.Entity<Product>().HasData(new Product { ProductId = 3, ProductName = "Iphone 13", Price = 1300, IsActive = true });
            modelBuilder.Entity<Product>().HasData(new Product { ProductId = 4, ProductName = "Iphone 14", Price = 1400, IsActive = true });

        }
    }
}
