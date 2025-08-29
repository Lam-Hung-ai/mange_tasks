namespace mange_tasks.Server.Entities;

public class TaskPriority
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
