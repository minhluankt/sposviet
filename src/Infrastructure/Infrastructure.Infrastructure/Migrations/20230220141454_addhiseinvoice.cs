using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class addhiseinvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "EInvoice",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "HistoryEInvoice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusEvent = table.Column<int>(type: "int", nullable: false),
                    IdEInvoice = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EInvoiceCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Carsher = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IdCarsher = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryEInvoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryEInvoice_EInvoice_IdEInvoice",
                        column: x => x.IdEInvoice,
                        principalTable: "EInvoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoryEInvoice_IdEInvoice",
                table: "HistoryEInvoice",
                column: "IdEInvoice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoryEInvoice");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "EInvoice");
        }
    }
}
