using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class lstIdAndNameCategoryShowInHome : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pageSize",
                table: "ConfigSystem");

            migrationBuilder.DropColumn(
                name: "pageSizeInHome",
                table: "ConfigSystem");

            migrationBuilder.RenameColumn(
                name: "lstIdAndNameCategoryShowInHome",
                table: "ConfigSystem",
                newName: "Value");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "ConfigSystem",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "ConfigSystem");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "ConfigSystem",
                newName: "lstIdAndNameCategoryShowInHome");

            migrationBuilder.AddColumn<int>(
                name: "pageSize",
                table: "ConfigSystem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "pageSizeInHome",
                table: "ConfigSystem",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
