using ApiCourseSltn2.DTO;
using ApiCourseSltn2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApiCourseSltn2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
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
    }
}
