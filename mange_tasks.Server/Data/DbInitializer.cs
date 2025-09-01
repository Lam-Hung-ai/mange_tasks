using Microsoft.EntityFrameworkCore;
using mange_tasks.Server.Entities;
using mange_tasks.Server.Authorization;
namespace mange_tasks.Server.Data;

public static class DbInitializer
{

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await ctx.Database.MigrateAsync();
        await SeedPermissionsAsync(ctx);
        await SeedRolesAsync(ctx);
        await SeedRolePermissionsAsync(ctx);
        await SeedTaskPrioritiesAsync(ctx);
        await SeedProjectStatusesForAllProjectsAsync(ctx);
    }

    private static async Task SeedPermissionsAsync(AppDbContext ctx)
    {
        // Lấy danh sách tất cả các hằng số quyền từ lớp Permissions
        var allPermissions = typeof(Permissions)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .Select(f => (string)f.GetValue(null)!)
            .ToList();

        // Lấy các quyền đã có trong DB để tránh trùng lặp
        var existingPermissions = await ctx.Permissions
            .Select(p => p.Name)
            .ToHashSetAsync();

        var newPermissions = new List<Permission>();
        foreach (var permissionName in allPermissions)
        {
            if (!existingPermissions.Contains(permissionName))
            {
                newPermissions.Add(new Permission { Name = permissionName });
            }
        }

        if (newPermissions.Any())
        {
            await ctx.Permissions.AddRangeAsync(newPermissions);
            await ctx.SaveChangesAsync();
        }
    }

    private static async Task SeedRolesAsync(AppDbContext ctx)
    {
        var rolesToSeed = new List<Role>
        {
            new() { Name = Roles.Admin, Scope = RoleScopes.System },
            new() { Name = Roles.User, Scope = RoleScopes.System },
            new() { Name = Roles.Viewer, Scope = RoleScopes.System },
            new() { Name = Roles.ProjectOwner, Scope = RoleScopes.Project },
            new() { Name = Roles.ProjectManager, Scope = RoleScopes.Project },
            new() { Name = Roles.ProjectMember, Scope = RoleScopes.Project }
        };

        var existingRoleNames = await ctx.Roles.Select(r => r.Name).ToHashSetAsync();
        var newRoles = rolesToSeed.Where(r => !existingRoleNames.Contains(r.Name)).ToList();

        if (newRoles.Any())
        {
            await ctx.Roles.AddRangeAsync(newRoles);
            await ctx.SaveChangesAsync();
        }
    }

    private static async Task SeedRolePermissionsAsync(AppDbContext ctx)
    {
        // Định nghĩa các mapping ở đây
        var rolePermissionMappings = new Dictionary<string, List<string>>
        {
            // System Roles
            [Roles.Admin] = new()
            {
                Permissions.ManageUsers,
                Permissions.ManageRoles,
                Permissions.AccessAll
            },
            [Roles.User] = new()
            {
                Permissions.CreateProject,
                Permissions.ReadMember // Để có thể thấy user khác và mời vào project
            },
            [Roles.Viewer] = new()
            {
                Permissions.ReadProject,
                Permissions.ReadTask,
                Permissions.ReadMember,
                Permissions.ReadComment
            },

            // Project Roles
            [Roles.ProjectOwner] = new()
            {
                // Toàn quyền của Manager
                Permissions.UpdateProject, Permissions.CreateTask, Permissions.ReadTask, Permissions.UpdateAllTasks,
                Permissions.DeleteAllTasks, Permissions.AssignTask, Permissions.InviteMember, Permissions.RemoveMember,
                Permissions.UpdateAllComments, Permissions.DeleteAllComments,
                
                // Quyền riêng của Owner
                Permissions.DeleteProject,
                Permissions.UpdateMemberRole
            },
            [Roles.ProjectManager] = new()
            {
                Permissions.UpdateProject,
                Permissions.CreateTask,
                Permissions.ReadTask,
                Permissions.UpdateAllTasks,
                Permissions.DeleteAllTasks,
                Permissions.AssignTask,
                Permissions.InviteMember,
                Permissions.RemoveMember,
                // Quản lý comment
                Permissions.CreateComment,
                Permissions.ReadComment,
                Permissions.UpdateAllComments,
                Permissions.DeleteAllComments
            },
            [Roles.ProjectMember] = new()
            {
                Permissions.CreateTask,
                Permissions.ReadTask,
                Permissions.UpdateOwnTasks,
                Permissions.DeleteOwnTasks,
                Permissions.AssignTask,
                Permissions.InviteMember, // Tùy chọn, có thể cho phép hoặc không
                // Quyền comment cơ bản
                Permissions.CreateComment,
                Permissions.ReadComment,
                Permissions.UpdateOwnComment,
                Permissions.DeleteOwnComment
            }
        };

        // Lấy tất cả roles và permissions từ DB vào bộ nhớ để xử lý nhanh
        var roles = await ctx.Roles.ToDictionaryAsync(r => r.Name, r => r.Id);
        var permissions = await ctx.Permissions.ToDictionaryAsync(p => p.Name, p => p.Id);

        // Lấy các cặp Role-Permission đã có để tránh trùng lặp
        var existingRolePermissions = await ctx.RolePermissions
            .Select(rp => new { rp.RoleId, rp.PermissionId })
            .ToHashSetAsync();

        var newRolePermissions = new List<RolePermission>();

        foreach (var mapping in rolePermissionMappings)
        {
            var roleName = mapping.Key;
            var permissionNames = mapping.Value;

            if (!roles.TryGetValue(roleName, out var roleId)) continue;

            foreach (var permissionName in permissionNames)
            {
                if (!permissions.TryGetValue(permissionName, out var permissionId)) continue;

                // Nếu cặp này chưa tồn tại trong DB, thêm mới
                if (!existingRolePermissions.Contains(new { RoleId = roleId, PermissionId = permissionId }))
                {
                    newRolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
                }
            }
        }

        // Thêm quyền đọc cơ bản mà mọi role dự án đều có
        var projectRoles = new List<string> { Roles.ProjectOwner, Roles.ProjectManager, Roles.ProjectMember };
        var basicReadPermissions = new List<string> { Permissions.ReadProject, Permissions.ReadTask, Permissions.ReadMember, Permissions.ReadComment };

        foreach (var roleName in projectRoles)
        {
            if (!roles.TryGetValue(roleName, out var roleId)) continue;

            foreach (var permissionName in basicReadPermissions)
            {
                if (!permissions.TryGetValue(permissionName, out var permissionId)) continue;
                if (!existingRolePermissions.Contains(new { RoleId = roleId, PermissionId = permissionId }))
                {
                    var existingMapping = newRolePermissions.FirstOrDefault(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
                    if (existingMapping == null)
                    {
                        newRolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
                    }
                }
            }
        }

        if (newRolePermissions.Any())
        {
            await ctx.RolePermissions.AddRangeAsync(newRolePermissions);
            await ctx.SaveChangesAsync();
        }
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