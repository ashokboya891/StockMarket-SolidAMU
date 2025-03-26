using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarketSolution.Migrations
{
    public partial class relation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "SellOrders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "BuyOrders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SellOrders_UserID",
                table: "SellOrders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_BuyOrders_UserID",
                table: "BuyOrders",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_BuyOrders_AspNetUsers_UserID",
                table: "BuyOrders",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SellOrders_AspNetUsers_UserID",
                table: "SellOrders",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuyOrders_AspNetUsers_UserID",
                table: "BuyOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SellOrders_AspNetUsers_UserID",
                table: "SellOrders");

            migrationBuilder.DropIndex(
                name: "IX_SellOrders_UserID",
                table: "SellOrders");

            migrationBuilder.DropIndex(
                name: "IX_BuyOrders_UserID",
                table: "BuyOrders");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "SellOrders");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "BuyOrders");
        }
    }
}
