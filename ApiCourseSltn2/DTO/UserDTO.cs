using System.ComponentModel.DataAnnotations;

namespace ApiCourseSltn2.DTO
{
    public class UserDTO
    {
        //productDTO benzeri bir yapı zaten, LoginViewModel a da benziyor.
        [Required]
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string FullName { get; set; } = null!;
    }
}
