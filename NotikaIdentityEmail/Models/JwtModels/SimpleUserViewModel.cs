
namespace NotikaIdentityEmail.Models.JwtModels
{
    public class SimpleUserViewModel
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Token { get; set; } = null!;

    }
}
