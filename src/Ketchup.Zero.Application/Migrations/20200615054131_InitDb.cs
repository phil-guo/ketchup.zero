using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ketchup.Zero.Application.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_menu",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: true),
                    Url = table.Column<string>(maxLength: 20, nullable: false),
                    Level = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    Operates = table.Column<string>(maxLength: 100, nullable: true),
                    Sort = table.Column<int>(nullable: false),
                    Icon = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_operate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    Remark = table.Column<string>(maxLength: 2147483647, nullable: true),
                    Unique = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_operate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_role",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    Remark = table.Column<string>(maxLength: 2147483647, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_roleMenu",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(nullable: false),
                    MenuId = table.Column<int>(nullable: false),
                    Operates = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_roleMenu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_user",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(maxLength: 32, nullable: false),
                    Password = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "sys_menu",
                columns: new[] { "Id", "Icon", "Level", "Name", "Operates", "ParentId", "Sort", "Url" },
                values: new object[,]
                {
                    { 1, null, (sbyte)1, "基础数据管理", "[]", 0, 0, "/" },
                    { 2, null, (sbyte)2, "按钮管理", "[1,2,3,4]", 1, 1, "operate" },
                    { 3, null, (sbyte)2, "角色管理", "[1,2,3,4]", 1, 2, "role" },
                    { 4, null, (sbyte)2, "菜单管理", "[1,2,3,4]", 1, 3, "menu" },
                    { 5, null, (sbyte)2, "系统用户", "[1,2,3,4]", 1, 4, "sysUser" }
                });

            migrationBuilder.InsertData(
                table: "sys_operate",
                columns: new[] { "Id", "Name", "Remark", "Unique" },
                values: new object[,]
                {
                    { 1, "添加", null, 1001 },
                    { 2, "编辑", null, 1002 },
                    { 3, "查询", null, 1003 },
                    { 4, "删除", null, 1004 }
                });

            migrationBuilder.InsertData(
                table: "sys_role",
                columns: new[] { "Id", "Name", "Remark" },
                values: new object[] { 1, "管理员", null });

            migrationBuilder.InsertData(
                table: "sys_roleMenu",
                columns: new[] { "Id", "MenuId", "Operates", "RoleId" },
                values: new object[,]
                {
                    { 1, 1, "[]", 1 },
                    { 2, 2, "[1,2,3,4]", 1 },
                    { 3, 3, "[1,2,3]", 1 },
                    { 4, 4, "[1,2,3,4]", 1 }
                });

            migrationBuilder.InsertData(
                table: "sys_user",
                columns: new[] { "Id", "Password", "RoleId", "UserName" },
                values: new object[] { 1, "46F94C8DE14FB36680850768FF1B7F2A", 1, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_menu");

            migrationBuilder.DropTable(
                name: "sys_operate");

            migrationBuilder.DropTable(
                name: "sys_role");

            migrationBuilder.DropTable(
                name: "sys_roleMenu");

            migrationBuilder.DropTable(
                name: "sys_user");
        }
    }
}
