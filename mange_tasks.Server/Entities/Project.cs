namespace mange_tasks.Server.Entities;

public class Project
{
    public long Id { get; set; }
    public Guid PublicId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    public ICollection<TaskStatus> Statuses { get; set; } = new List<TaskStatus>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
