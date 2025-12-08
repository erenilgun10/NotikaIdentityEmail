namespace NotikaIdentityEmail.Models.MessageViewModels
{
    public class MessageWithSenderInfoViewModel
    {
        public int MessageId { get; set; }
        public int? CategoryId { get; set; }
        public string? Subject { get; set; }
        public string? MessageDetail { get; set; }
        public DateTime SendDate { get; set; }
        public string? SenderEmail { get; set; }
        public string? SenderFirstName { get; set; }
        public string? SenderLastName { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryLabelFormat { get; set; }



    }
}
