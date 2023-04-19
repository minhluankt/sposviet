using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class addGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderTable_Id_OrderTableCode",
                table: "OrderTable");

            migrationBuilder.AddColumn<Guid>(
                name: "IdGuid",
                table: "RoomAndTable",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IdGuid",
                table: "OrderTable",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IdGuid",
                table: "Invoice",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RoomAndTable_IdGuid",
                table: "RoomAndTable",
                column: "IdGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_Id_OrderTableCode_IdGuid",
                table: "OrderTable",
                columns: new[] { "Id", "OrderTableCode", "IdGuid" },
                unique: true,
                filter: "[OrderTableCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_IdGuid",
                table: "Invoice",
                column: "IdGuid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomAndTable_IdGuid",
                table: "RoomAndTable");

            migrationBuilder.DropIndex(
                name: "IX_OrderTable_Id_OrderTableCode_IdGuid",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_IdGuid",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "IdGuid",
                table: "RoomAndTable");

            migrationBuilder.DropColumn(
                name: "IdGuid",
                table: "OrderTable");

            migrationBuilder.DropColumn(
                name: "IdGuid",
                table: "Invoice");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_Id_OrderTableCode",
                table: "OrderTable",
                columns: new[] { "Id", "OrderTableCode" },
                unique: true,
                filter: "[OrderTableCode] IS NOT NULL");
        }
    }
}
