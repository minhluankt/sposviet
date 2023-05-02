using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class addCategoryInvoiceTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCategoryInvoiceTemplate",
                table: "TemplateInvoice",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CategoryInvoiceTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryInvoiceTemplate", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateInvoice_IdCategoryInvoiceTemplate",
                table: "TemplateInvoice",
                column: "IdCategoryInvoiceTemplate");
             
            migrationBuilder.CreateIndex(
                name: "IX_CategoryInvoiceTemplate_Slug",
                table: "CategoryInvoiceTemplate",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateInvoice_CategoryInvoiceTemplate_IdCategoryInvoiceTemplate",
                table: "TemplateInvoice",
                column: "IdCategoryInvoiceTemplate",
                principalTable: "CategoryInvoiceTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemplateInvoice_CategoryInvoiceTemplate_IdCategoryInvoiceTemplate",
                table: "TemplateInvoice");

            migrationBuilder.DropTable(
                name: "CategoryInvoiceTemplate");

            migrationBuilder.DropIndex(
                name: "IX_TemplateInvoice_IdCategoryInvoiceTemplate",
                table: "TemplateInvoice");

            migrationBuilder.DropColumn(
                name: "IdCategoryInvoiceTemplate",
                table: "TemplateInvoice");
        }
    }
}
