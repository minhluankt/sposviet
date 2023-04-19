using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatett : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomAndTable_IdGuid",
                table: "RoomAndTable");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "RoomAndTable",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AmountChangeCus",
                table: "Invoice",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AmountCusPayment",
                table: "Invoice",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomAndTable_IdGuid_Slug",
                table: "RoomAndTable",
                columns: new[] { "IdGuid", "Slug" },
                unique: true,
                filter: "[Slug] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomAndTable_IdGuid_Slug",
                table: "RoomAndTable");

            migrationBuilder.DropColumn(
                name: "AmountChangeCus",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "AmountCusPayment",
                table: "Invoice");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "RoomAndTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomAndTable_IdGuid",
                table: "RoomAndTable",
                column: "IdGuid",
                unique: true);
        }
    }
}
