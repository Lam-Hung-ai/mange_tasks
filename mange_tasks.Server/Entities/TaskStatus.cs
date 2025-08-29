namespace mange_tasks.Server.Entities;

public class TaskStatus
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public bool IsDefault { get; set; }
    public bool IsFinal { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
