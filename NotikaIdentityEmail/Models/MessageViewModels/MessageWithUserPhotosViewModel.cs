using NotikaIdentityEmail.Entities;

namespace NotikaIdentityEmail.Models.MessageViewModels
{
    public class MessageWithUserPhotosViewModel
    {
        public string? FullName { get; set; }
        public string? ImageUrl { get; set; }
        public Message? Message { get; set; }   

    }
}
