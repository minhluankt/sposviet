using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class IsUniqueupdatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TemplateInvoice_Id_Slug",
                table: "TemplateInvoice");

            migrationBuilder.DropIndex(
                name: "IX_TableLink_ID_slug",
                table: "TableLink");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_Id_Slug",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_RoomAndTable_IdGuid_Slug",
                table: "RoomAndTable");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrder_Id_Code",
                table: "PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_Product_IdCategory_ViewNumber_isPromotion",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_OrderTable_Id_OrderTableCode_IdGuid",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_OrderTable_TypeInvoice_TypeProduct_ComId",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_ManagerPatternEInvoice_TypeSupplierEInvoice_ComId",
                table: "ManagerPatternEInvoice");

            migrationBuilder.DropIndex(
                name: "IX_Kitchen_Id_IdKitchen",
                table: "Kitchen");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_TypeInvoice_TypeProduct_ComId",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_IdInvoice_CusCode_ComId",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_ContentPromotionProduct_IdProduct_Id",
                table: "ContentPromotionProduct");

            migrationBuilder.DropIndex(
                name: "IX_CompanyAdminInfo_VFkeyCusTaxCode_VFkeyPhone",
                table: "CompanyAdminInfo");

            migrationBuilder.DropIndex(
                name: "IX_CategoryCevenue_Code_Slug",
                table: "CategoryCevenue");

            migrationBuilder.DropIndex(
                name: "IX_Area_Id_Slug",
                table: "Area");

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
                name: "IX_PurchaseOrder_Code",
                table: "PurchaseOrder",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Product_IdCategory",
                table: "Product",
                column: "IdCategory");

            migrationBuilder.CreateIndex(
                name: "IX_Product_isPromotion",
                table: "Product",
                column: "isPromotion");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ViewNumber",
                table: "Product",
                column: "ViewNumber");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_ComId",
                table: "OrderTable",
                column: "ComId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_IdGuid",
                table: "OrderTable",
                column: "IdGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_OrderTableCode",
                table: "OrderTable",
                column: "OrderTableCode",
                unique: true,
                filter: "[OrderTableCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_TypeInvoice",
                table: "OrderTable",
                column: "TypeInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_TypeProduct",
                table: "OrderTable",
                column: "TypeProduct");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerPatternEInvoice_ComId",
                table: "ManagerPatternEInvoice",
                column: "ComId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerPatternEInvoice_TypeSupplierEInvoice",
                table: "ManagerPatternEInvoice",
                column: "TypeSupplierEInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_IdKitchen",
                table: "Kitchen",
                column: "IdKitchen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ComId",
                table: "Invoice",
                column: "ComId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_TypeInvoice",
                table: "Invoice",
                column: "TypeInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_TypeProduct",
                table: "Invoice",
                column: "TypeProduct");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_ComId",
                table: "EInvoice",
                column: "ComId");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_CusCode",
                table: "EInvoice",
                column: "CusCode");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_IdInvoice",
                table: "EInvoice",
                column: "IdInvoice");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPromotionProduct_IdProduct",
                table: "ContentPromotionProduct",
                column: "IdProduct");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAdminInfo_VFkeyCusTaxCode",
                table: "CompanyAdminInfo",
                column: "VFkeyCusTaxCode",
                unique: true,
                filter: "[VFkeyCusTaxCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAdminInfo_VFkeyPhone",
                table: "CompanyAdminInfo",
                column: "VFkeyPhone",
                unique: true,
                filter: "[VFkeyPhone] IS NOT NULL");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "IX_PurchaseOrder_Code",
                table: "PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_Product_IdCategory",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_isPromotion",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_ViewNumber",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_OrderTable_ComId",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_OrderTable_IdGuid",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_OrderTable_OrderTableCode",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_OrderTable_TypeInvoice",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_OrderTable_TypeProduct",
                table: "OrderTable");

            migrationBuilder.DropIndex(
                name: "IX_ManagerPatternEInvoice_ComId",
                table: "ManagerPatternEInvoice");

            migrationBuilder.DropIndex(
                name: "IX_ManagerPatternEInvoice_TypeSupplierEInvoice",
                table: "ManagerPatternEInvoice");

            migrationBuilder.DropIndex(
                name: "IX_Kitchen_IdKitchen",
                table: "Kitchen");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_ComId",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_TypeInvoice",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_TypeProduct",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_ComId",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_CusCode",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_EInvoice_IdInvoice",
                table: "EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_ContentPromotionProduct_IdProduct",
                table: "ContentPromotionProduct");

            migrationBuilder.DropIndex(
                name: "IX_CompanyAdminInfo_VFkeyCusTaxCode",
                table: "CompanyAdminInfo");

            migrationBuilder.DropIndex(
                name: "IX_CompanyAdminInfo_VFkeyPhone",
                table: "CompanyAdminInfo");

            migrationBuilder.DropIndex(
                name: "IX_CategoryCevenue_Slug",
                table: "CategoryCevenue");

            migrationBuilder.DropIndex(
                name: "IX_Area_Slug",
                table: "Area");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateInvoice_Id_Slug",
                table: "TemplateInvoice",
                columns: new[] { "Id", "Slug" },
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TableLink_ID_slug",
                table: "TableLink",
                columns: new[] { "ID", "slug" },
                unique: true,
                filter: "[slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Id_Slug",
                table: "Suppliers",
                columns: new[] { "Id", "Slug" },
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAndTable_IdGuid_Slug",
                table: "RoomAndTable",
                columns: new[] { "IdGuid", "Slug" },
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_Id_Code",
                table: "PurchaseOrder",
                columns: new[] { "Id", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Product_IdCategory_ViewNumber_isPromotion",
                table: "Product",
                columns: new[] { "IdCategory", "ViewNumber", "isPromotion" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_Id_OrderTableCode_IdGuid",
                table: "OrderTable",
                columns: new[] { "Id", "OrderTableCode", "IdGuid" },
                unique: true,
                filter: "[OrderTableCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_TypeInvoice_TypeProduct_ComId",
                table: "OrderTable",
                columns: new[] { "TypeInvoice", "TypeProduct", "ComId" });

            migrationBuilder.CreateIndex(
                name: "IX_ManagerPatternEInvoice_TypeSupplierEInvoice_ComId",
                table: "ManagerPatternEInvoice",
                columns: new[] { "TypeSupplierEInvoice", "ComId" });

            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_Id_IdKitchen",
                table: "Kitchen",
                columns: new[] { "Id", "IdKitchen" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_TypeInvoice_TypeProduct_ComId",
                table: "Invoice",
                columns: new[] { "TypeInvoice", "TypeProduct", "ComId" });

            migrationBuilder.CreateIndex(
                name: "IX_EInvoice_IdInvoice_CusCode_ComId",
                table: "EInvoice",
                columns: new[] { "IdInvoice", "CusCode", "ComId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentPromotionProduct_IdProduct_Id",
                table: "ContentPromotionProduct",
                columns: new[] { "IdProduct", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAdminInfo_VFkeyCusTaxCode_VFkeyPhone",
                table: "CompanyAdminInfo",
                columns: new[] { "VFkeyCusTaxCode", "VFkeyPhone" },
                unique: true,
                filter: "[VFkeyCusTaxCode] IS NOT NULL AND [VFkeyPhone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCevenue_Code_Slug",
                table: "CategoryCevenue",
                columns: new[] { "Code", "Slug" },
                unique: true,
                filter: "[Code] IS NOT NULL AND [Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Area_Id_Slug",
                table: "Area",
                columns: new[] { "Id", "Slug" },
                unique: true,
                filter: "[Slug] IS NOT NULL");
        }
    }
}
