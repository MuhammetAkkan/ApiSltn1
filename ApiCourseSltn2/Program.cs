using ApiCourseSltn2.Controllers;
using ApiCourseSltn2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ProductsContext>(connection => connection.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

//Identity kendi başına bir hizmet olduğundan servis kısmında bunu eklemeliyiz.
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<ProductsContext>();

builder.Services.Configure<IdentityOptions>(options => 
{
    //Password settings
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;


    //user related settings - user a yönelik ayarlar/şartlar
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcçdefghıijklmnoöpqrsştuüvwxyzABCÇDEFGHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789-._@+";
    

    //hesap kilitleme ayarları
    options.Lockout.AllowedForNewUsers= true;
    options.Lockout.MaxFailedAccessAttempts = 5; //maximum 5 kere yanlış girebilir.


    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //yanlış giriş sonrası kilitleme süresi
    



});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
