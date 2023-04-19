using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class ApplicationInitialHistoryKitchen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoryKitchen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdKitchen = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IdCashername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cashername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCancel = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryKitchen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryKitchen_Kitchen_IdKitchen",
                        column: x => x.IdKitchen,
                        principalTable: "Kitchen",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoryKitchen_Id",
                table: "HistoryKitchen",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoryKitchen_IdKitchen",
                table: "HistoryKitchen",
                column: "IdKitchen");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoryKitchen");
        }
    }
}
