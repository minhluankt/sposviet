using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class ApplicationInitialArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdArea",
                table: "RoomAndTable",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComId = table.Column<int>(type: "int", nullable: false),
                    IdGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomAndTable_IdArea",
                table: "RoomAndTable",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_Area_Id_Slug",
                table: "Area",
                columns: new[] { "Id", "Slug" },
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAndTable_Area_IdArea",
                table: "RoomAndTable",
                column: "IdArea",
                principalTable: "Area",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomAndTable_Area_IdArea",
                table: "RoomAndTable");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropIndex(
                name: "IX_RoomAndTable_IdArea",
                table: "RoomAndTable");

            migrationBuilder.DropColumn(
                name: "IdArea",
                table: "RoomAndTable");
        }
    }
}
