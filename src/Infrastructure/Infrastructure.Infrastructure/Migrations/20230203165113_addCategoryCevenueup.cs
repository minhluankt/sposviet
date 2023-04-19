using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class addCategoryCevenueup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CategoryCevenue_Code",
                table: "CategoryCevenue");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CategoryCevenue",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "CategoryCevenue",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCevenue_Code_Slug",
                table: "CategoryCevenue",
                columns: new[] { "Code", "Slug" },
                unique: true,
                filter: "[Code] IS NOT NULL AND [Slug] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CategoryCevenue_Code_Slug",
                table: "CategoryCevenue");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "CategoryCevenue");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CategoryCevenue",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCevenue_Code",
                table: "CategoryCevenue",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }
    }
}
