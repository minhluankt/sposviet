using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class UpdateNotifyProcessingFood : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateProcessing",
                table: "Kitchen",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HistoryKitchen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdKitchen = table.Column<int>(type: "int", nullable: false),
                    StaffName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryKitchen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryKitchen_Kitchen_IdKitchen",
                        column: x => x.IdKitchen,
                        principalTable: "Kitchen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoryKitchen_IdKitchen",
                table: "HistoryKitchen",
                column: "IdKitchen");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoryKitchen");

            migrationBuilder.DropColumn(
                name: "DateProcessing",
                table: "Kitchen");
        }
    }
}
