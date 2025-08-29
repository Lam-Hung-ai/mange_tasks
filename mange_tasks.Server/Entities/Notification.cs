namespace mange_tasks.Server.Entities;

public class Notification
{
    public long Id { get; set; }
    public long RecipientUserId { get; set; }
    public User RecipientUser { get; set; } = null!;
    public long? ActorUserId { get; set; }
    public User? ActorUser { get; set; }
    public string? EntityType { get; set; }
    public long? EntityId { get; set; }
    public string? NotificationType { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
