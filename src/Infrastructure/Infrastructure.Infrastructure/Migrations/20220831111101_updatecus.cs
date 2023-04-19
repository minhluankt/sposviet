using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class updatecus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            //migrationBuilder.DropIndex(
            //    name: "IX_Order_CustomerId",
            //    table: "Order");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_Order_IdCustomer",
                table: "Order",
                column: "IdCustomer");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Customer_IdCustomer",
                table: "Order",
                column: "IdCustomer",
                principalTable: "Customer",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Customer_IdCustomer",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_IdCustomer",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Order",
                type: "int",
                nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Order_CustomerId",
            //    table: "Order",
            //    column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Customer_IdCustomer",
                table: "Order",
                column: "IdCustomer",
                principalTable: "Customer",
                principalColumn: "Id");
        }
    }
}
