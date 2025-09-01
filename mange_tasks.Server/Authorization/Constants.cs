namespace mange_tasks.Server.Authorization
{
    public static class RoleScopes
    {
        public const string System = "System";
        public const string Project = "Project";
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Viewer = "Viewer";
        public const string ProjectOwner = "ProjectOwner";
        public const string ProjectManager = "ProjectManager";
        public const string ProjectMember = "ProjectMember";
    }

    public static class Permissions
    {
        // Project Permissions
        public const string CreateProject = "project:create";
        public const string ReadProject = "project:read";
        public const string UpdateProject = "project:update";
        public const string DeleteProject = "project:delete";

        // Task Permissions
        public const string CreateTask = "task:create";
        public const string ReadTask = "task:read";
        public const string UpdateAllTasks = "task:update:all";
        public const string UpdateOwnTasks = "task:update:own";
        public const string DeleteAllTasks = "task:delete:all";
        public const string DeleteOwnTasks = "task:delete:own";
        public const string AssignTask = "task:assign";

        // Project Member Permissions
        public const string InviteMember = "member:invite";
        public const string RemoveMember = "member:remove";
        public const string ReadMember = "member:read";
        public const string UpdateMemberRole = "member:update_role";

        // Comment Permissions
        public const string CreateComment = "comment:create";
        public const string ReadComment = "comment:read";
        public const string UpdateOwnComment = "comment:update:own";
        public const string DeleteOwnComment = "comment:delete:own";
        public const string UpdateAllComments = "comment:update:all";
        public const string DeleteAllComments = "comment:delete:all";

        // Admin Permissions
        public const string ManageUsers = "admin:manage_users";
        public const string ManageRoles = "admin:manage_roles";
        public const string AccessAll = "admin:access_all";
    }
}
