using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updateconffigsys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "ConfigSystem",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComId",
                table: "ConfigSystem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ConfigSystem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ConfigSystem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Parent",
                table: "ConfigSystem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ConfigSystem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConfigSystem_Key_ComId",
                table: "ConfigSystem",
                columns: new[] { "Key", "ComId" },
                unique: true,
                filter: "[Key] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConfigSystem_Key_ComId",
                table: "ConfigSystem");

            migrationBuilder.DropColumn(
                name: "ComId",
                table: "ConfigSystem");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ConfigSystem");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "ConfigSystem");

            migrationBuilder.DropColumn(
                name: "Parent",
                table: "ConfigSystem");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ConfigSystem");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "ConfigSystem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
