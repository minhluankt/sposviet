using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class themsuplereinvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EInvoice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeSupplierEInvoice = table.Column<int>(type: "int", nullable: false),
                    ComId = table.Column<int>(type: "int", nullable: false),
                    IdInvoice = table.Column<int>(type: "int", nullable: false),
                    Fkey = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    InvoiceNo = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Pattern = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Serial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CasherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArisingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrencyUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountInWords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CusBankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CusBankNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CusPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Buyer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CusCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CCCD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmailDeliver = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CusName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CusTaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<float>(type: "real", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TGTKCThue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TGTKCKhac = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VATAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VATRate = table.Column<float>(type: "real", nullable: false),
                    StatusEinvoice = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EInvoice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplierEInvoice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComId = table.Column<int>(type: "int", nullable: false),
                    TypeSupplierEInvoice = table.Column<int>(type: "int", nullable: false),
                    DomainName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserNameAdmin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassWordAdmin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserNameService = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassWordService = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierEInvoice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EInvoiceItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdInvoice = table.Column<int>(type: "int", nullable: false),
                    IsSum = table.Column<int>(type: "int", nullable: false),
                    ProCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VATRate = table.Column<float>(type: "real", nullable: false),
                    VATAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<float>(type: "real", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EInvoiceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EInvoiceItem_EInvoice_IdInvoice",
                        column: x => x.IdInvoice,
                        principalTable: "EInvoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagerPatternEInvoice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeSupplierEInvoice = table.Column<int>(type: "int", nullable: false),
                    ComId = table.Column<int>(type: "int", nullable: false),
                    IdSupplierEInvoice = table.Column<int>(type: "int", nullable: false),
                    Pattern = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Serial = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VFkey = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerPatternEInvoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagerPatternEInvoice_SupplierEInvoice_IdSupplierEInvoice",
                        column: x => x.IdSupplierEInvoice,
                        principalTable: "SupplierEInvoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_Fkey",
                table: "EInvoice",
                column: "Fkey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_InvoiceNo_IdInvoice_ComId",
                table: "EInvoice",
                columns: new[] { "InvoiceNo", "IdInvoice", "ComId" });

            migrationBuilder.CreateIndex(
                name: "IX_EInvoiceItem_IdInvoice",
                table: "EInvoiceItem",
                column: "IdInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerPatternEInvoice_IdSupplierEInvoice",
                table: "ManagerPatternEInvoice",
                column: "IdSupplierEInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerPatternEInvoice_TypeSupplierEInvoice_ComId",
                table: "ManagerPatternEInvoice",
                columns: new[] { "TypeSupplierEInvoice", "ComId" });

            migrationBuilder.CreateIndex(
                name: "IX_ManagerPatternEInvoice_VFkey",
                table: "ManagerPatternEInvoice",
                column: "VFkey",
                unique: true,
                filter: "[VFkey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierEInvoice_TypeSupplierEInvoice_ComId",
                table: "SupplierEInvoice",
                columns: new[] { "TypeSupplierEInvoice", "ComId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EInvoiceItem");

            migrationBuilder.DropTable(
                name: "ManagerPatternEInvoice");

            migrationBuilder.DropTable(
                name: "EInvoice");

            migrationBuilder.DropTable(
                name: "SupplierEInvoice");
        }
    }
}
