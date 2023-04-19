using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatecolumnComponentProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeProductCategory",
                table: "ComponentProduct",
                newName: "TypeComponentProduct");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeComponentProduct",
                table: "ComponentProduct",
                newName: "TypeProductCategory");
        }
    }
}
