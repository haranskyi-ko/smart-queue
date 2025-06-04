namespace QueueApp.Shared.Models
{
    public class QueueCreateModel
    {
        public string Title { get; set; } = string.Empty;
        public bool AllowMultipleEntries { get; set; }
    }
}