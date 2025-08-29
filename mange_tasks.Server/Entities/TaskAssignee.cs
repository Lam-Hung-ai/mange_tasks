namespace mange_tasks.Server.Entities;

public class TaskAssignee
{
    public long TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public int AssignmentType { get; set; } // 1 = Primary, 2 = Collaborator
}
