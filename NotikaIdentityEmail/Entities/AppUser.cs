using Microsoft.AspNetCore.Identity;

namespace NotikaIdentityEmail.Entities
{
    public class AppUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ImageUrl { get; set; }
        public string? City { get; set; }
    }
}
