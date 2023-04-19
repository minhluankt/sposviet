using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class ENumTypeManagerInvd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ManagerInvNo_Type",
                table: "ManagerInvNo");

            migrationBuilder.AddColumn<string>(
                name: "VFkey",
                table: "ManagerInvNo",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManagerInvNo_VFkey",
                table: "ManagerInvNo",
                column: "VFkey",
                unique: true,
                filter: "[VFkey] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ManagerInvNo_VFkey",
                table: "ManagerInvNo");

            migrationBuilder.DropColumn(
                name: "VFkey",
                table: "ManagerInvNo");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerInvNo_Type",
                table: "ManagerInvNo",
                column: "Type",
                unique: true);
        }
    }
}
