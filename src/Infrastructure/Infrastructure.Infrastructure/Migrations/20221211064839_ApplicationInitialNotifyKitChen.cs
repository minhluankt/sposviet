using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Migrations
{
    public partial class ApplicationInitialNotifyKitChen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TableName",
                table: "Kitchen",
                newName: "RoomTableName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Kitchen",
                newName: "ProName");

            migrationBuilder.AlterColumn<int>(
                name: "IdProduct",
                table: "Kitchen",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Buyer",
                table: "Kitchen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdRoomTable",
                table: "Kitchen",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBingBack",
                table: "Kitchen",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Buyer",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "IdRoomTable",
                table: "Kitchen");

            migrationBuilder.DropColumn(
                name: "IsBingBack",
                table: "Kitchen");

            migrationBuilder.RenameColumn(
                name: "RoomTableName",
                table: "Kitchen",
                newName: "TableName");

            migrationBuilder.RenameColumn(
                name: "ProName",
                table: "Kitchen",
                newName: "Name");

            migrationBuilder.AlterColumn<int>(
                name: "IdProduct",
                table: "Kitchen",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
