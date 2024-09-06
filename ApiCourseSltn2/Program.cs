using ApiCourseSltn2.Controllers;
using ApiCourseSltn2.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Add Serives
var myCorsPolicy = "MyCorsPoliciy";

//Add CORS => Microsoft
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myCorsPolicy,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7145/",
                                             "https://localhost:7033/",
                                             "http://example2.com")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});



// Add services IdentityDdContext
builder.Services.AddDbContext<ProductsContext>(connection => connection.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
//BURASI

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Varsayılan kimlik doğrulama şeması olarak JWT Bearer kullanılır.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Kimlik doğrulama zorluğu için kullanılacak varsayılan şema.

    /*
     * opsiyonel
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme; // Oturum açma işlemleri için kullanılacak varsayılan şema.
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme; // Oturum kapatma işlemleri için kullanılacak varsayılan şema.
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // Genel varsayılan kimlik doğrulama şeması.
    */
})
.AddJwtBearer(options =>
{
    //istekler sadece HTTPS den mi gelsin?
    options.RequireHttpsMetadata = false; // HTTPS kullanımını zorunlu hale getirir.

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // Token'ın geçerli bir issuer'dan yani firma/site vb geldiğini doğrular.
        ValidIssuer = "muhammetAkkan.com", // Geçerli issuer.

        ValidateAudience = false, // Token'ın geçerli bir audience'a sahip olduğunu doğrular.
        ValidAudience = "", // Geçerli audience.

        ValidateLifetime = true, // Token'ın süresinin dolmadığını doğrular.

        //bu önemli
        ValidateIssuerSigningKey = true, // Token'ın imzasının geçerli olduğunu doğrular.

        //kullanımı yukarıdakine bağlı.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("AppSettings:Secret").Value ?? "")) // İmza için kullanılan gizli anahtar.
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            // Kimlik doğrulama başarısız olduğunda yapılacak işlemler.
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // Token doğrulandığında yapılacak işlemler.
            return Task.CompletedTask;
        }
    };

   
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Swagerı özelleştirme      
builder.Services.AddSwaggerGen(options =>
{
    // API bilgilerini ekleyin
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
    });

    // Authentication için Swagger konfigürasyonu
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",


    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });


    


    /*
    // API endpoint'leri için açıklamalar ve örnekler ekleyebilirsiniz
    options.OperationFilter<AddResponseHeadersOperationFilter>();
    options.OperationFilter<AddAuthorizationHeaderOperationFilter>();
    */

    // XML yorumlarını dahil et (Opsiyonel)
    /*
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    */
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.UseAuthentication(); //JWT

app.UseRouting();

//Cors policy use Start
app.UseCors(myCorsPolicy);
//Cors policy use End

app.UseAuthorization();

app.MapControllers();

app.Run();
