using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XFLCSMS.Migrations
{
    public partial class UpdateClosedby : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApproveBy",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApproveOn",
                table: "Issues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClosedBy",
                table: "Issues",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "Issues",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveBy",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "ApproveOn",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "Issues");
        }
    }
}
