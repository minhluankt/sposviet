using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class OptionsName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAddingOptions",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "OptionsDetailtProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOptionsName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdProduct = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionsDetailtProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionsDetailtProduct_Product_IdProduct",
                        column: x => x.IdProduct,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StyleOptionsProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StyleOptionsProduct", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StyleProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProduct = table.Column<int>(type: "int", nullable: false),
                    IdStyleOptionsProduct = table.Column<int>(type: "int", nullable: false),
                    Sort = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StyleProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StyleProduct_Product_IdProduct",
                        column: x => x.IdProduct,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StyleProduct_StyleOptionsProduct_IdStyleOptionsProduct",
                        column: x => x.IdStyleOptionsProduct,
                        principalTable: "StyleOptionsProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OptionsName",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdStyleProduct = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionsName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionsName_StyleProduct_IdStyleProduct",
                        column: x => x.IdStyleProduct,
                        principalTable: "StyleProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptionsDetailtProduct_IdProduct",
                table: "OptionsDetailtProduct",
                column: "IdProduct");

            migrationBuilder.CreateIndex(
                name: "IX_OptionsName_IdStyleProduct",
                table: "OptionsName",
                column: "IdStyleProduct");

            migrationBuilder.CreateIndex(
                name: "IX_StyleProduct_IdProduct_IdStyleOptionsProduct",
                table: "StyleProduct",
                columns: new[] { "IdProduct", "IdStyleOptionsProduct" });

            migrationBuilder.CreateIndex(
                name: "IX_StyleProduct_IdStyleOptionsProduct",
                table: "StyleProduct",
                column: "IdStyleOptionsProduct");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptionsDetailtProduct");

            migrationBuilder.DropTable(
                name: "OptionsName");

            migrationBuilder.DropTable(
                name: "StyleProduct");

            migrationBuilder.DropTable(
                name: "StyleOptionsProduct");

            migrationBuilder.DropColumn(
                name: "IsAddingOptions",
                table: "Product");
        }
    }
}
