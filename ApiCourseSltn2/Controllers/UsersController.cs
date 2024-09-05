using ApiCourseSltn2.DTO;
using ApiCourseSltn2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiCourseSltn2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //DI Start
        private UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        
        //appSettingsten Key bilgisini alabilmemiz için IConfugration kullanırız.
        private readonly IConfiguration _configuration; 

        public UsersController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configurtaion)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configurtaion;
        }

        //DI End

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            
            var isUsers = await _userManager.Users.Select(i=> new {i.FullName, i.Email}).ToListAsync();

            if (isUsers is null || !isUsers.Any())
                return NotFound();

            

            return Ok(isUsers);
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(UserDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email!);

            user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,

                //UserDTO da DateAdded yok lakin user da var, dolayısıyla model de erişilemiyor ama biz user a ekleyebiliyoruz.
                /*
                 * burada dikkat edilmesi gereken kapsam => User bigger UserDTO
                 * UserDTO yı sadece kullanıcı görüyor, ve müdahale edebileceği alanları biz belirliyoruz.
                */
                DateAdded = DateTime.Now,
            };

            //bu metot userName i ve Email i aynı yapmana engel oluyor.
            var result = await _userManager.CreateAsync(user, model.Password ?? "12345");

            if (result.Succeeded)
            {
                return StatusCode(statusCode: 201, $"{model.FullName}");
            }

            return BadRequest(result.Errors);

        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return BadRequest(new {message="Bu email adresine kayıtlı kullanıcı yok."});

            //Burada kimlik doğrulaması yapıyoruz lakin oturum açmıyoruz.
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                //token oluşturma JWT
                return Ok(new 
                {
                    token = GenerateJWT(user),
                });
            }

            //yetkisiz deneme
            return Unauthorized("Hata!");
        }

        //JWT Methot at here write
        //YAZILMAYA DEVAM EDİLECEK, ŞUAN EKSİK. => 04.09.2024
        private object GenerateJWT(AppUser user)
        {
            
            /*
             * AppSettings i biz kendi isim terciğimizle AppSettings.Development.json a yazdık.
             * Secret adlı isimlendirmeyi de aynı şekilde kendimiz yaptık.
             * Encoding.ASCII.GetBytes => alınan/gelen(get) string anahtarı 
            */
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value ?? "");

            //tokenDescriptor => token tanımlayıcı(kimlik)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Token ın özneleri => Subject
                Subject = new ClaimsIdentity(new Claim[]
                {
                    //Id ye denk geliyor lakin NameIdentifier(source a bak) string olduğundan kendi user.Id mizi de string e çeviriyoruz.
                    new (ClaimTypes.NameIdentifier  , user.Id.ToString()), 

                    //login vb durumlar için userName i kullandığımızdan ClamiTypes ı userName ile ilişkilendirdik.
                    new (ClaimTypes.Name, user.UserName ?? ""),
                }),

                //Expires = süresi dolma
                //Expires satırında jwt nin süresini belirliyoruz.
                Expires = DateTime.UtcNow.AddDays(1), //1 gün sonra süresi bitecek

                //JWT nin kimlik bilgilerini içerir. Yapısını ve şifrelenme türünü içerir.
                //Önemli olarak SigningCredentials jwt güvenlidir, değiştirilmedi. Bu algoritma ve imza     ile de teyit ediyorum demektir. 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = "muhammetAkkan.com",
                //SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256, "RS256") vb de kullanılabilirdi.
            };

            /*
             * bir token üretebilmem için => 1 adet token yöneticisine(token handler)
             *                            => 1 adet token kimlik bilgileri(tokenDescriptor)
             * bir token ı kullanabilme için => 1 adet oluşturulmuş token ı yazdırmalı ve kullanıma göre(return vb) almalıyım.
             */

            //token:token && handler:yönetici
            var tokenHandler = new JwtSecurityTokenHandler();


            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var finalyTokenProsess = tokenHandler.WriteToken(token);
            return finalyTokenProsess;
        }

    }
}
