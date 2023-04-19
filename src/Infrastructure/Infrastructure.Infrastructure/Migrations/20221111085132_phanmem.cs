using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class phanmem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBarcode",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrintItem",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KitchenId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalesChannel",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Kitchen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitchen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomAndTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STT = table.Column<short>(type: "smallint", nullable: false),
                    NumberSeats = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAndTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeSupplier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsBringBack = table.Column<bool>(type: "bit", nullable: false),
                    InvoiceCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Buyer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdOrderTable = table.Column<int>(type: "int", nullable: true),
                    IdCustomer = table.Column<int>(type: "int", nullable: true),
                    IdRoomAndTable = table.Column<int>(type: "int", nullable: true),
                    IsRetailCustomer = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCasher = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IdPaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<float>(type: "real", nullable: false),
                    DiscountAmount = table.Column<double>(type: "float", nullable: false),
                    DeliveryCharges = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    Amonut = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoice_Customer_IdCustomer",
                        column: x => x.IdCustomer,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoice_PaymentMethod_IdPaymentMethod",
                        column: x => x.IdPaymentMethod,
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoice_RoomAndTable_IdRoomAndTable",
                        column: x => x.IdRoomAndTable,
                        principalTable: "RoomAndTable",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderSort = table.Column<int>(type: "int", nullable: false),
                    OrderTableCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsBringBack = table.Column<bool>(type: "bit", nullable: false),
                    Buyer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdInvoice = table.Column<int>(type: "int", nullable: true),
                    IdCustomer = table.Column<int>(type: "int", nullable: true),
                    IdRoomAndTable = table.Column<int>(type: "int", nullable: true),
                    IsRetailCustomer = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCasher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdPaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<float>(type: "real", nullable: false),
                    DiscountAmount = table.Column<double>(type: "float", nullable: false),
                    Amonut = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTable_Customer_IdCustomer",
                        column: x => x.IdCustomer,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderTable_PaymentMethod_IdPaymentMethod",
                        column: x => x.IdPaymentMethod,
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTable_RoomAndTable_IdRoomAndTable",
                        column: x => x.IdRoomAndTable,
                        principalTable: "RoomAndTable",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdInvoice = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<float>(type: "real", nullable: false),
                    DiscountAmount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceItem_Invoice_IdInvoice",
                        column: x => x.IdInvoice,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdInvoice = table.Column<int>(type: "int", nullable: true),
                    IdSuppliers = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DebtAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DisCount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DisCountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Carsher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_Invoice_IdInvoice",
                        column: x => x.IdInvoice,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_Suppliers_IdSuppliers",
                        column: x => x.IdSuppliers,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HistoryOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrderTable = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Carsher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsNotif = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryOrder_OrderTable_IdOrderTable",
                        column: x => x.IdOrderTable,
                        principalTable: "OrderTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderTableItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrderTable = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<float>(type: "real", nullable: false),
                    DiscountAmount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTableItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTableItem_OrderTable_IdOrderTable",
                        column: x => x.IdOrderTable,
                        principalTable: "OrderTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemPurchaseOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPurchaseOrder = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<float>(type: "real", nullable: false),
                    DiscountAmount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPurchaseOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemPurchaseOrder_PurchaseOrder_IdPurchaseOrder",
                        column: x => x.IdPurchaseOrder,
                        principalTable: "PurchaseOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_KitchenId",
                table: "Product",
                column: "KitchenId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryOrder_IdOrderTable",
                table: "HistoryOrder",
                column: "IdOrderTable");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_Id_InvoiceCode",
                table: "Invoice",
                columns: new[] { "Id", "InvoiceCode" },
                unique: true,
                filter: "[InvoiceCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_IdCustomer",
                table: "Invoice",
                column: "IdCustomer");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_IdPaymentMethod",
                table: "Invoice",
                column: "IdPaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_IdRoomAndTable",
                table: "Invoice",
                column: "IdRoomAndTable");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_IdInvoice",
                table: "InvoiceItem",
                column: "IdInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPurchaseOrder_IdPurchaseOrder",
                table: "ItemPurchaseOrder",
                column: "IdPurchaseOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_Id_Code",
                table: "Kitchen",
                columns: new[] { "Id", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_Id_OrderTableCode",
                table: "OrderTable",
                columns: new[] { "Id", "OrderTableCode" },
                unique: true,
                filter: "[OrderTableCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_IdCustomer",
                table: "OrderTable",
                column: "IdCustomer");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_IdPaymentMethod",
                table: "OrderTable",
                column: "IdPaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_IdRoomAndTable",
                table: "OrderTable",
                column: "IdRoomAndTable");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTableItem_IdOrderTable",
                table: "OrderTableItem",
                column: "IdOrderTable");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_Id_Code",
                table: "PurchaseOrder",
                columns: new[] { "Id", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_IdInvoice",
                table: "PurchaseOrder",
                column: "IdInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_IdSuppliers",
                table: "PurchaseOrder",
                column: "IdSuppliers");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Id_Slug",
                table: "Suppliers",
                columns: new[] { "Id", "Slug" },
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Kitchen_KitchenId",
                table: "Product",
                column: "KitchenId",
                principalTable: "Kitchen",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Kitchen_KitchenId",
                table: "Product");

            migrationBuilder.DropTable(
                name: "HistoryOrder");

            migrationBuilder.DropTable(
                name: "InvoiceItem");

            migrationBuilder.DropTable(
                name: "ItemPurchaseOrder");

            migrationBuilder.DropTable(
                name: "Kitchen");

            migrationBuilder.DropTable(
                name: "OrderTableItem");

            migrationBuilder.DropTable(
                name: "PurchaseOrder");

            migrationBuilder.DropTable(
                name: "OrderTable");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "RoomAndTable");

            migrationBuilder.DropIndex(
                name: "IX_Product_KitchenId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsBarcode",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsPrintItem",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "KitchenId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SalesChannel",
                table: "Product");
        }
    }
}
