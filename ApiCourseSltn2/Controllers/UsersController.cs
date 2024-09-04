using ApiCourseSltn2.DTO;
using ApiCourseSltn2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

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
                    token = "Token Gönderildi"
                });
            }

            //yetkisiz deneme
            return Unauthorized("Hata!");
        }

        //JWT Methot at here write
        //YAZILMAYA DEVAM EDİLECEK, ŞUAN EKSİK. => 04.09.2024
        private object GenerateJWT(AppUser appUser)
        {
            //token:token && handler:yönetici
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler;
        }

    }
}
