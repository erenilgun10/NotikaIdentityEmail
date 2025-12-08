namespace NotikaIdentityEmail.Models.MessageViewModels
{
    public class MessageWithReceiverInfoViewModel
    {
        public int MessageId { get; set; }
        public string? Subject { get; set; }
        public string? MessageDetail { get; set; }
        public DateTime SendDate { get; set; }
        public string? ReceiverEmail { get; set; }
        public string? ReceiverFirstName { get; set; }
        public string? ReceiverLastName { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryLabelFormat { get; set; }



    }
}
