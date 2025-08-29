using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mange_tasks.Server.Entities;

namespace mange_tasks.Server.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure DB is created/migrated
        await ctx.Database.MigrateAsync();

        await SeedPermissionsAsync(ctx);
        await SeedRolesAsync(ctx);
        await SeedRolePermissionsAsync(ctx);
        await SeedTaskPrioritiesAsync(ctx);
        await SeedProjectStatusesForAllProjectsAsync(ctx);
    }

    private static async Task SeedPermissionsAsync(AppDbContext ctx)
    {
        if (await ctx.Permissions.AnyAsync()) return;
        var permissions = new[]
        {
            "CreateTask","UpdateTask","DeleteTask","ViewTask",
            "AssignTask","CommentTask","UploadAttachment",
            "CreateProject","UpdateProject","DeleteProject","ViewProject",
            "ManageMembers","ManageRoles","ManageTags","ManageStatuses"
        };
        foreach (var name in permissions.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            ctx.Permissions.Add(new Permission { Name = name });
        }
        await ctx.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(AppDbContext ctx)
    {
        if (await ctx.Roles.AnyAsync()) return;
        var roles = new[] { "Admin", "Member", "Viewer" };
        foreach (var name in roles)
        {
            ctx.Roles.Add(new Role { Name = name });
        }
        await ctx.SaveChangesAsync();
    }

    private static async Task SeedRolePermissionsAsync(AppDbContext ctx)
    {
        // If already has any role-permissions, skip
        if (await ctx.RolePermissions.AnyAsync()) return;

        var allPerms = await ctx.Permissions.Select(p => p.Id).ToListAsync();
        var admin = await ctx.Roles.FirstAsync(r => r.Name == "Admin");
        foreach (var pid in allPerms)
        {
            ctx.RolePermissions.Add(new RolePermission { RoleId = admin.Id, PermissionId = pid });
        }

        var member = await ctx.Roles.FirstAsync(r => r.Name == "Member");
        var memberPerms = await ctx.Permissions
            .Where(p => new[]
            {
                "CreateTask","UpdateTask","DeleteTask","ViewTask",
                "AssignTask","CommentTask","UploadAttachment",
                "ViewProject","ManageTags"
            }.Contains(p.Name))
            .Select(p => p.Id)
            .ToListAsync();
        foreach (var pid in memberPerms)
        {
            ctx.RolePermissions.Add(new RolePermission { RoleId = member.Id, PermissionId = pid });
        }

        var viewer = await ctx.Roles.FirstAsync(r => r.Name == "Viewer");
        var viewerPerms = await ctx.Permissions
            .Where(p => new[] { "ViewTask", "ViewProject" }.Contains(p.Name))
            .Select(p => p.Id)
            .ToListAsync();
        foreach (var pid in viewerPerms)
        {
            ctx.RolePermissions.Add(new RolePermission { RoleId = viewer.Id, PermissionId = pid });
        }

        await ctx.SaveChangesAsync();
    }

    private static async Task SeedTaskPrioritiesAsync(AppDbContext ctx)
    {
        if (await ctx.TaskPriorities.AnyAsync()) return;
        var priorities = new[]
        {
            new TaskPriority { Name = "High", Order = 1 },
            new TaskPriority { Name = "Medium", Order = 2 },
            new TaskPriority { Name = "Low", Order = 3 }
        };
        ctx.TaskPriorities.AddRange(priorities);
        await ctx.SaveChangesAsync();
    }

    // Default statuses in English only
    private static readonly (string Name, int Order, bool IsDefault, bool IsFinal)[] DefaultEnStatuses = new[]
    {
        ("Todo", 1, true, false),
        ("InProgress", 2, false, false),
        ("Review", 3, false, false),
        ("Done", 4, false, true),
        ("Blocked", 5, false, false),
        ("Cancelled", 6, false, true)
    };

    private static async Task SeedProjectStatusesForAllProjectsAsync(AppDbContext ctx)
    {
        var projectIds = await ctx.Projects.Select(p => p.Id).ToListAsync();
        foreach (var pid in projectIds)
        {
            var statuses = await ctx.TaskStatuses.Where(s => s.ProjectId == pid).ToListAsync();
            if (statuses.Count == 0)
            {
                foreach (var (name, order, isDefault, isFinal) in DefaultEnStatuses)
                {
                    ctx.TaskStatuses.Add(new mange_tasks.Server.Entities.TaskStatus
                    {
                        Name = name,
                        Order = order,
                        ProjectId = pid,
                        IsDefault = isDefault,
                        IsFinal = isFinal
                    });
                }
                continue;
            }

            // Ensure all default English statuses exist with correct properties
            foreach (var (name, order, isDefault, isFinal) in DefaultEnStatuses)
            {
                var existing = statuses.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (existing is null)
                {
                    ctx.TaskStatuses.Add(new mange_tasks.Server.Entities.TaskStatus
                    {
                        Name = name,
                        Order = order,
                        ProjectId = pid,
                        IsDefault = isDefault,
                        IsFinal = isFinal
                    });
                }
                else
                {
                    existing.Order = order;
                    existing.IsDefault = isDefault;
                    existing.IsFinal = isFinal;
                }
            }

            // Ensure only Todo is default among existing ones
            foreach (var s in statuses)
            {
                s.IsDefault = s.Name.Equals("Todo", StringComparison.OrdinalIgnoreCase);
            }
        }

        if (ctx.ChangeTracker.HasChanges())
        {
            await ctx.SaveChangesAsync();
        }
    }
}
