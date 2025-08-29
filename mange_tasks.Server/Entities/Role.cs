namespace mange_tasks.Server.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
}
