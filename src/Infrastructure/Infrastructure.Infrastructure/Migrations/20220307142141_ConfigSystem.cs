using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class ConfigSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ListIdCategoryProduct",
                table: "ConfigSystem",
                newName: "lstIdAndNameCategoryShowInHome");

            migrationBuilder.AddColumn<int>(
                name: "pageSizeInHome",
                table: "ConfigSystem",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pageSizeInHome",
                table: "ConfigSystem");

            migrationBuilder.RenameColumn(
                name: "lstIdAndNameCategoryShowInHome",
                table: "ConfigSystem",
                newName: "ListIdCategoryProduct");
        }
    }
}
