namespace mange_tasks.Server.Entities;

public class Tag
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
