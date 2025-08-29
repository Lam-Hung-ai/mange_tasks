namespace mange_tasks.Server.Entities;

public class Comment
{
    public long Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public long TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
