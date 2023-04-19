using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class IsUniqueupdatabaselan2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TemplateInvoice_Slug",
                table: "TemplateInvoice");

            migrationBuilder.DropIndex(
                name: "IX_TableLink_slug",
                table: "TableLink");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_Slug",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_RoomAndTable_IdGuid",
                table: "RoomAndTable");

            migrationBuilder.DropIndex(
                name: "IX_RoomAndTable_Slug",
                table: "RoomAndTable");

            migrationBuilder.DropIndex(
                name: "IX_RevenueExpenditure_Code",
                table: "RevenueExpenditure");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrder_Code",
                table: "PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_ParametersEmail_Key",
                table: "ParametersEmail");

            migrationBuilder.DropIndex(
                name: "IX_OrderTable_OrderTableCode",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_Kitchen_IdKitchen",
                table: "Kitchen");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_Id_InvoiceCode",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_IdGuid",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_Fkey",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_InvoiceCode",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_MCQT",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_DetailtKitchen_Id",
                table: "DetailtKitchen");

            migrationBuilder.DropIndex(
                name: "IX_CategoryCevenue_Slug",
                table: "CategoryCevenue");

            migrationBuilder.DropIndex(
                name: "IX_Area_Slug",
                table: "Area");

            migrationBuilder.AddColumn<int>(
                name: "Comid",
                table: "PurchaseOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ComId",
                table: "ParametersEmail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Customer",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateInvoice_Slug_ComId",
                table: "TemplateInvoice",
                columns: new[] { "Slug", "ComId" },
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TableLink_slug_tableId",
                table: "TableLink",
                columns: new[] { "slug", "tableId" },
                unique: true,
                filter: "[slug] IS NOT NULL AND [tableId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Slug_ComId",
                table: "Suppliers",
                columns: new[] { "Slug", "ComId" },
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAndTable_IdGuid_ComId",
                table: "RoomAndTable",
                columns: new[] { "IdGuid", "ComId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomAndTable_Slug_ComId_IdArea",
                table: "RoomAndTable",
                columns: new[] { "Slug", "ComId", "IdArea" },
                unique: true,
                filter: "[Slug] IS NOT NULL AND [IdArea] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueExpenditure_Code_ComId",
                table: "RevenueExpenditure",
                columns: new[] { "Code", "ComId" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_Code_Comid",
                table: "PurchaseOrder",
                columns: new[] { "Code", "Comid" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Code_ComId",
                table: "Product",
                columns: new[] { "Code", "ComId" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ParametersEmail_Key_ComId",
                table: "ParametersEmail",
                columns: new[] { "Key", "ComId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_OrderTableCode_ComId",
                table: "OrderTable",
                columns: new[] { "OrderTableCode", "ComId" },
                unique: true,
                filter: "[OrderTableCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_IdKitchen_ComId",
                table: "Kitchen",
                columns: new[] { "IdKitchen", "ComId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ComId_InvoiceCode",
                table: "Invoice",
                columns: new[] { "ComId", "InvoiceCode" },
                unique: true,
                filter: "[InvoiceCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_Fkey_ComId",
                table: "EInvoice",
                columns: new[] { "Fkey", "ComId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_InvoiceCode_ComId",
                table: "EInvoice",
                columns: new[] { "InvoiceCode", "ComId" },
                unique: true,
                filter: "[InvoiceCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_MCQT_ComId",
                table: "EInvoice",
                columns: new[] { "MCQT", "ComId" },
                unique: true,
                filter: "[MCQT] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Code_Comid",
                table: "Customer",
                columns: new[] { "Code", "Comid" },
                unique: true,
                filter: "[Code] IS NOT NULL AND [Comid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProduct_ComId_Code",
                table: "CategoryProduct",
                columns: new[] { "ComId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCevenue_Slug_ComId",
                table: "CategoryCevenue",
                columns: new[] { "Slug", "ComId" },
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Area_Slug_ComId",
                table: "Area",
                columns: new[] { "Slug", "ComId" },
                unique: true,
                filter: "[Slug] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TemplateInvoice_Slug_ComId",
                table: "TemplateInvoice");

            migrationBuilder.DropIndex(
                name: "IX_TableLink_slug_tableId",
                table: "TableLink");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_Slug_ComId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_RoomAndTable_IdGuid_ComId",
                table: "RoomAndTable");

            migrationBuilder.DropIndex(
                name: "IX_RoomAndTable_Slug_ComId_IdArea",
                table: "RoomAndTable");

            migrationBuilder.DropIndex(
                name: "IX_RevenueExpenditure_Code_ComId",
                table: "RevenueExpenditure");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrder_Code_Comid",
                table: "PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_Product_Code_ComId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_ParametersEmail_Key_ComId",
                table: "ParametersEmail");

            migrationBuilder.DropIndex(
                name: "IX_OrderTable_OrderTableCode_ComId",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_Kitchen_IdKitchen_ComId",
                table: "Kitchen");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_ComId_InvoiceCode",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_Fkey_ComId",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_InvoiceCode_ComId",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_MCQT_ComId",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Code_Comid",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_CategoryProduct_ComId_Code",
                table: "CategoryProduct");

            migrationBuilder.DropIndex(
                name: "IX_CategoryCevenue_Slug_ComId",
                table: "CategoryCevenue");

            migrationBuilder.DropIndex(
                name: "IX_Area_Slug_ComId",
                table: "Area");

            migrationBuilder.DropColumn(
                name: "Comid",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "ComId",
                table: "ParametersEmail");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateInvoice_Slug",
                table: "TemplateInvoice",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TableLink_slug",
                table: "TableLink",
                column: "slug",
                unique: true,
                filter: "[slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Slug",
                table: "Suppliers",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAndTable_IdGuid",
                table: "RoomAndTable",
                column: "IdGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomAndTable_Slug",
                table: "RoomAndTable",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueExpenditure_Code",
                table: "RevenueExpenditure",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_Code",
                table: "PurchaseOrder",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ParametersEmail_Key",
                table: "ParametersEmail",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_OrderTableCode",
                table: "OrderTable",
                column: "OrderTableCode",
                unique: true,
                filter: "[OrderTableCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_IdKitchen",
                table: "Kitchen",
                column: "IdKitchen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_Id_InvoiceCode",
                table: "Invoice",
                columns: new[] { "Id", "InvoiceCode" },
                unique: true,
                filter: "[InvoiceCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_IdGuid",
                table: "Invoice",
                column: "IdGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_Fkey",
                table: "EInvoice",
                column: "Fkey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_InvoiceCode",
                table: "EInvoice",
                column: "InvoiceCode",
                unique: true,
                filter: "[InvoiceCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_MCQT",
                table: "EInvoice",
                column: "MCQT",
                unique: true,
                filter: "[MCQT] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DetailtKitchen_Id",
                table: "DetailtKitchen",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCevenue_Slug",
                table: "CategoryCevenue",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Area_Slug",
                table: "Area",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");
        }
    }
}
