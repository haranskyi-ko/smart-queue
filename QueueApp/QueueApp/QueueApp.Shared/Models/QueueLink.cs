// QueueLink.cs
public class QueueLink
{
    public int Id { get; set; }
    public string UniqueCode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool AllowMultipleEntries { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<QueueItem> Items { get; set; } = new List<QueueItem>();
}
