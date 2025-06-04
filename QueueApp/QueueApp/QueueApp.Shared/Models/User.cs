// User.cs
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime RegistrationTime { get; set; }

    public ICollection<QueueItem> QueueItems { get; set; } = new List<QueueItem>();
}
