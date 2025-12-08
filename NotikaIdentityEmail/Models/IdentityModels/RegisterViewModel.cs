namespace NotikaIdentityEmail.Models.IdentityModels
{
    public class RegisterViewModel
    {
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; } = null!;
        public string? ConfirmPassword { get; set; }
    }
}
