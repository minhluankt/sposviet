using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class addHtmlQrCodeVietQR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HtmlQrCodeVietQR",
                table: "TemplateInvoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowQrCodeVietQR",
                table: "TemplateInvoice",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HtmlQrCodeVietQR",
                table: "TemplateInvoice");

            migrationBuilder.DropColumn(
                name: "IsShowQrCodeVietQR",
                table: "TemplateInvoice");
        }
    }
}
