using Models.Enums_queue.Models;

public class QueueItem
{
    public int Id { get; set; }

    /* --- FK до QueueLink --- */
    public int QueueLinkId { get; set; }
    public QueueLink QueueLink { get; set; } = null!;

    /* --- FK до User --- */
    public int UserId { get; set; }          // ← ЛИШЕ ОДИН (!) FK-стовпчик
    public User User { get; set; } = null!;  //   і одна навігація

    public DateTime EnqueuedAt { get; set; }
    public QueueStatus Status { get; set; }
}
