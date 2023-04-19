using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class categorypost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Category_IdCategory",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_IdCategory",
                table: "Product");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.CreateTable(
                name: "CategoryPost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdLevel = table.Column<int>(type: "int", nullable: false),
                    IdPattern = table.Column<int>(type: "int", nullable: true),
                    IdTypeCategory = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryPost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryPost_CategoryPost_IdPattern",
                        column: x => x.IdPattern,
                        principalTable: "CategoryPost",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategoryPost_TypeCategory_IdTypeCategory",
                        column: x => x.IdTypeCategory,
                        principalTable: "TypeCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdLevel = table.Column<int>(type: "int", nullable: false),
                    IdPattern = table.Column<int>(type: "int", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryProduct_CategoryProduct_IdPattern",
                        column: x => x.IdPattern,
                        principalTable: "CategoryProduct",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPost_IdPattern",
                table: "CategoryPost",
                column: "IdPattern");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPost_IdTypeCategory",
                table: "CategoryPost",
                column: "IdTypeCategory");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProduct_IdPattern",
                table: "CategoryProduct",
                column: "IdPattern");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_CategoryPost_IdCategory",
                table: "Post",
                column: "IdCategory",
                principalTable: "CategoryPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_CategoryProduct_IdCategory",
                table: "Product",
                column: "IdCategory",
                principalTable: "CategoryProduct",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_CategoryPost_IdCategory",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_CategoryProduct_IdCategory",
                table: "Product");

            migrationBuilder.DropTable(
                name: "CategoryPost");

            migrationBuilder.DropTable(
                name: "CategoryProduct");

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPattern = table.Column<int>(type: "int", nullable: true),
                    IdTypeCategory = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdLevel = table.Column<int>(type: "int", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_Category_IdPattern",
                        column: x => x.IdPattern,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Category_TypeCategory_IdTypeCategory",
                        column: x => x.IdTypeCategory,
                        principalTable: "TypeCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_IdPattern",
                table: "Category",
                column: "IdPattern");

            migrationBuilder.CreateIndex(
                name: "IX_Category_IdTypeCategory",
                table: "Category",
                column: "IdTypeCategory");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Category_IdCategory",
                table: "Post",
                column: "IdCategory",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_IdCategory",
                table: "Product",
                column: "IdCategory",
                principalTable: "Category",
                principalColumn: "Id");
        }
    }
}
