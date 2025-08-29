namespace mange_tasks.Server.Entities;

public class TaskTag
{
    public long TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;

    public long TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
