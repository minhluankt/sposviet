using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class addGuid2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdGuid",
                table: "OrderTableItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OrderTableItem_IdGuid",
                table: "OrderTableItem",
                column: "IdGuid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderTableItem_IdGuid",
                table: "OrderTableItem");

            migrationBuilder.DropColumn(
                name: "IdGuid",
                table: "OrderTableItem");
        }
    }
}
