namespace mange_tasks.Server.Entities;

public class Attachment
{
    public long Id { get; set; }
    public long? ProjectId { get; set; }
    public Project? Project { get; set; }
    public long? TaskId { get; set; }
    public TaskItem? Task { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long? FileSize { get; set; }
    public string? FileType { get; set; }
    public long UploadedBy { get; set; }
    public User UploadedByUser { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
