using Microsoft.EntityFrameworkCore.Migrations;

namespace Ketchup.Zero.Application.Migrations
{
    public partial class UpdataSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "sys_menu",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Icon", "ParentId" },
                values: new object[] { "folder-o", 99999 });

            migrationBuilder.UpdateData(
                table: "sys_menu",
                keyColumn: "Id",
                keyValue: 2,
                column: "Icon",
                value: "folder-o");

            migrationBuilder.UpdateData(
                table: "sys_menu",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Icon", "Operates" },
                values: new object[] { "folder-o", "[1,2,3,4,5]" });

            migrationBuilder.UpdateData(
                table: "sys_menu",
                keyColumn: "Id",
                keyValue: 4,
                column: "Icon",
                value: "folder-o");

            migrationBuilder.UpdateData(
                table: "sys_menu",
                keyColumn: "Id",
                keyValue: 5,
                column: "Icon",
                value: "folder-o");

            migrationBuilder.InsertData(
                table: "sys_operate",
                columns: new[] { "Id", "Name", "Remark", "Unique" },
                values: new object[] { 5, "权限", null, 1005 });

            migrationBuilder.UpdateData(
                table: "sys_roleMenu",
                keyColumn: "Id",
                keyValue: 3,
                column: "Operates",
                value: "[1,2,3,4,5]");

            migrationBuilder.InsertData(
                table: "sys_roleMenu",
                columns: new[] { "Id", "MenuId", "Operates", "RoleId" },
                values: new object[] { 5, 5, "[1,2,3,4]", 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "sys_operate",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "sys_roleMenu",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "sys_menu",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Icon", "ParentId" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "sys_menu",
                keyColumn: "Id",
                keyValue: 2,
                column: "Icon",
                value: null);

            migrationBuilder.UpdateData(
                table: "sys_menu",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Icon", "Operates" },
                values: new object[] { null, "[1,2,3,4]" });

            migrationBuilder.UpdateData(
                table: "sys_menu",
                keyColumn: "Id",
                keyValue: 4,
                column: "Icon",
                value: null);

            migrationBuilder.UpdateData(
                table: "sys_menu",
                keyColumn: "Id",
                keyValue: 5,
                column: "Icon",
                value: null);

            migrationBuilder.UpdateData(
                table: "sys_roleMenu",
                keyColumn: "Id",
                keyValue: 3,
                column: "Operates",
                value: "[1,2,3]");
        }
    }
}
