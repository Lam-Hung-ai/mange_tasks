namespace mange_tasks.Server.Entities;

public class ProjectMember
{
    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
