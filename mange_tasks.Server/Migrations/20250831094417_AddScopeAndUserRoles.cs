using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mange_tasks.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddScopeAndUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                       name: "Scope",
                       table: "Roles",
                       type: "VARCHAR(20)",
                       nullable: false,
                       defaultValue: "System");

            // 2. Tạo bảng UserRoles
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "BIGINT", nullable: false),
                    RoleId = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa bảng UserRoles
            migrationBuilder.DropTable(
                name: "UserRoles");

            // Xóa cột Scope trong Roles
            migrationBuilder.DropColumn(
                name: "Scope",
                table: "Roles");
        }
    }
}
