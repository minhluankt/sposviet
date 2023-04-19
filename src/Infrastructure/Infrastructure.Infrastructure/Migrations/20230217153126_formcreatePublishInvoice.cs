using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class formcreatePublishInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "InvoiceItem",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<decimal>(
                name: "Amonut",
                table: "InvoiceItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VATAmount",
                table: "InvoiceItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "VATRate",
                table: "InvoiceItem",
                type: "real",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Invoice",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "ServiceChargeAmount",
                table: "Invoice",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "Invoice",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryCharges",
                table: "Invoice",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountCusPayment",
                table: "Invoice",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountChangeCus",
                table: "Invoice",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amonut",
                table: "Invoice",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublishEInvoice",
                table: "Invoice",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "VATAmount",
                table: "Invoice",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "VATRate",
                table: "Invoice",
                type: "real",
                nullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Quantity",
                table: "EInvoiceItem",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MCQT",
                table: "EInvoice",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amonut",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "VATAmount",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "VATRate",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "IsPublishEInvoice",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "VATAmount",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "VATRate",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "MCQT",
                table: "EInvoice");

            migrationBuilder.AlterColumn<double>(
                name: "DiscountAmount",
                table: "InvoiceItem",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "Total",
                table: "Invoice",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "ServiceChargeAmount",
                table: "Invoice",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "DiscountAmount",
                table: "Invoice",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "DeliveryCharges",
                table: "Invoice",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "AmountCusPayment",
                table: "Invoice",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "AmountChangeCus",
                table: "Invoice",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Amonut",
                table: "Invoice",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Quantity",
                table: "EInvoiceItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
