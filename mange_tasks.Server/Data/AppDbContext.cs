using Microsoft.EntityFrameworkCore;
using mange_tasks.Server.Entities;

namespace mange_tasks.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<TaskPriority> TaskPriorities => Set<TaskPriority>();
    public DbSet<mange_tasks.Server.Entities.TaskStatus> TaskStatuses => Set<mange_tasks.Server.Entities.TaskStatus>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskAssignee> TaskAssignees => Set<TaskAssignee>();
    public DbSet<TaskTag> TaskTags => Set<TaskTag>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Permission>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Role>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<RolePermission>(e =>
        {
            e.HasKey(x => new { x.RoleId, x.PermissionId });
            e.HasOne(x => x.Role).WithMany(x => x.RolePermissions).HasForeignKey(x => x.RoleId);
            e.HasOne(x => x.Permission).WithMany(x => x.RolePermissions).HasForeignKey(x => x.PermissionId);
        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.PublicId).HasDefaultValueSql("NEWID()").IsRequired();
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.Property(x => x.UserName).HasMaxLength(256).IsRequired();
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasIndex(x => x.PublicId).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.UserName).IsUnique();
        });

        modelBuilder.Entity<Project>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.PublicId).HasDefaultValueSql("NEWID()").IsRequired();
            e.Property(x => x.ProjectCode).HasMaxLength(20).IsRequired();
            e.HasIndex(x => x.ProjectCode).IsUnique();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<ProjectMember>(e =>
        {
            e.HasKey(x => new { x.ProjectId, x.UserId });
            e.HasOne(x => x.Project).WithMany(x => x.Members).HasForeignKey(x => x.ProjectId);
            e.HasOne(x => x.User).WithMany(x => x.ProjectMemberships).HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Role).WithMany(x => x.ProjectMembers).HasForeignKey(x => x.RoleId);
        });

        modelBuilder.Entity<TaskPriority>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasMaxLength(50).IsRequired();
            e.Property(x => x.Order).IsRequired();
        });

        modelBuilder.Entity<mange_tasks.Server.Entities.TaskStatus>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasMaxLength(50).IsRequired();
            e.Property(x => x.Order).IsRequired();
            e.Property(x => x.IsDefault).HasDefaultValue(false);
            e.Property(x => x.IsFinal).HasDefaultValue(false);
            e.HasOne(x => x.Project).WithMany(x => x.Statuses).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaskItem>(e =>
        {
            e.ToTable("Tasks");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.PublicId).HasDefaultValueSql("NEWID()").IsRequired();
            e.Property(x => x.TaskCode).HasMaxLength(20).IsRequired();
            e.Property(x => x.Title).HasMaxLength(500).IsRequired();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasIndex(x => new { x.ProjectId, x.TaskCode }).IsUnique();
            e.HasOne(x => x.Status).WithMany(x => x.Tasks).HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Priority).WithMany(x => x.Tasks).HasForeignKey(x => x.PriorityId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Project).WithMany(x => x.Tasks).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.ParentTask).WithMany(x => x.SubTasks).HasForeignKey(x => x.ParentTaskId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<TaskAssignee>(e =>
        {
            e.HasKey(x => new { x.TaskId, x.UserId });
            e.Property(x => x.AssignmentType).IsRequired();
            e.HasOne(x => x.Task).WithMany(x => x.Assignees).HasForeignKey(x => x.TaskId);
            e.HasOne(x => x.User).WithMany(x => x.AssignedTasks).HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<TaskTag>(e =>
        {
            e.HasKey(x => new { x.TaskId, x.TagId });
            e.HasOne(x => x.Task).WithMany(x => x.TaskTags).HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.Tag).WithMany(x => x.TaskTags).HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Tag>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasMaxLength(50).IsRequired();
            e.HasOne(x => x.Project).WithMany(x => x.Tags).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Content).IsRequired();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasOne(x => x.Task).WithMany(x => x.Comments).HasForeignKey(x => x.TaskId);
            e.HasOne(x => x.User).WithMany(x => x.Comments).HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<Attachment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.FileName).HasMaxLength(255).IsRequired();
            e.Property(x => x.FilePath).HasMaxLength(1024).IsRequired();
            e.Property(x => x.UploadedAt).HasDefaultValueSql("GETDATE()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasOne(x => x.Project).WithMany(x => x.Attachments).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.Task).WithMany(x => x.Attachments).HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.NoAction);
            e.ToTable(t => t.HasCheckConstraint("CK_Attachments_ProjectOrTask", "ProjectId IS NOT NULL OR TaskId IS NOT NULL"));
        });

        modelBuilder.Entity<ActivityLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.EntityType).HasMaxLength(50);
            e.Property(x => x.ChangeType).HasMaxLength(100);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne(x => x.User).WithMany(x => x.ActivityLogs).HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<Notification>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.EntityType).HasMaxLength(50);
            e.Property(x => x.NotificationType).HasMaxLength(50);
            e.Property(x => x.IsRead).HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne(x => x.RecipientUser).WithMany(x => x.Notifications).HasForeignKey(x => x.RecipientUserId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.ActorUser).WithMany().HasForeignKey(x => x.ActorUserId).OnDelete(DeleteBehavior.NoAction);
        });
    }
}
