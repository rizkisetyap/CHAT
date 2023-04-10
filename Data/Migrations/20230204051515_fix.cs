using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Data.Migrations
{
    public partial class fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupChat_AspNetUsers_UserId",
                table: "GroupChat");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_GroupChat_GroupId",
                table: "UserGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupChat",
                table: "GroupChat");

            migrationBuilder.RenameTable(
                name: "UserGroups",
                newName: "UserGroup");

            migrationBuilder.RenameTable(
                name: "GroupChat",
                newName: "Group");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroup",
                newName: "IX_UserGroup_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupChat_UserId",
                table: "Group",
                newName: "IX_Group_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroup",
                table: "UserGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group",
                table: "Group",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Group_AspNetUsers_UserId",
                table: "Group",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroup_Group_GroupId",
                table: "UserGroup",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_AspNetUsers_UserId",
                table: "Group");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroup_Group_GroupId",
                table: "UserGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroup",
                table: "UserGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group",
                table: "Group");

            migrationBuilder.RenameTable(
                name: "UserGroup",
                newName: "UserGroups");

            migrationBuilder.RenameTable(
                name: "Group",
                newName: "GroupChat");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroup_GroupId",
                table: "UserGroups",
                newName: "IX_UserGroups_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Group_UserId",
                table: "GroupChat",
                newName: "IX_GroupChat_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupChat",
                table: "GroupChat",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChat_AspNetUsers_UserId",
                table: "GroupChat",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_GroupChat_GroupId",
                table: "UserGroups",
                column: "GroupId",
                principalTable: "GroupChat",
                principalColumn: "Id");
        }
    }
}
