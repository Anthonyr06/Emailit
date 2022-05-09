namespace Emailit.Models
{
    public class NewMessageNotification : Notification
    {
        public int MessageId { get; set; }
        public bool Confidential { get; set; }
        public MessagePriority Priority { get; set; }
        public bool ItHasAttachedFiles { get; set; }
    }
}
