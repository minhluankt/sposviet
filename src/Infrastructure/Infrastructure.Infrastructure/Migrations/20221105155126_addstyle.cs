using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class addstyle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StyleProduct_StyleOptionsProduct_IdStyleOptionsProduct",
                table: "StyleProduct");

            migrationBuilder.AddForeignKey(
                name: "FK_StyleProduct_Specifications_IdStyleOptionsProduct",
                table: "StyleProduct",
                column: "IdStyleOptionsProduct",
                principalTable: "Specifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StyleProduct_Specifications_IdStyleOptionsProduct",
                table: "StyleProduct");

            migrationBuilder.AddForeignKey(
                name: "FK_StyleProduct_StyleOptionsProduct_IdStyleOptionsProduct",
                table: "StyleProduct",
                column: "IdStyleOptionsProduct",
                principalTable: "StyleOptionsProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
