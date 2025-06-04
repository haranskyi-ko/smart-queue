namespace QueueApp.Shared.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }

}
