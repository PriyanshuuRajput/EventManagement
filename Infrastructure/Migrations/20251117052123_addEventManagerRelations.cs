using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addEventManagerRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerName",
                table: "Events");

            migrationBuilder.AlterColumn<int>(
                name: "ManagerId",
                table: "Events",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ManagerId",
                table: "Events",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_ManagerId",
                table: "Events",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_ManagerId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_ManagerId",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "ManagerId",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ManagerName",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
