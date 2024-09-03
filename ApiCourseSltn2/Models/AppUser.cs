using Microsoft.AspNetCore.Identity;

namespace ApiCourseSltn2.Models
{
    public class AppUser : IdentityUser<int> //AppUSER standart yine IdentiyUser dan kimliği miras alıyor. Bu sefer Id lerin guid değil integer olmasını istedik.
    {
        public string FullName { get; set; } = null!;
        public DateTime DateAdded { get; set; }
    }
}
