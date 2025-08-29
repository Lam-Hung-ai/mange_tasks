namespace mange_tasks.Server.Entities;

public class ActivityLog
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public User? User { get; set; }
    public string? EntityType { get; set; }
    public long? EntityId { get; set; }
    public string? ChangeType { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime CreatedAt { get; set; }
}
