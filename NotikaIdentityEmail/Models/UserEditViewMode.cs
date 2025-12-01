namespace NotikaIdentityEmail.Models
{
    public class UserEditViewModel
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PasswordConfirm { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string City { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

    }
}
