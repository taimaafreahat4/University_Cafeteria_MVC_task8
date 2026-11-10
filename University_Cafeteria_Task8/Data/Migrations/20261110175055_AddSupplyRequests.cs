
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Cafeteria_Task8.Data.Migrations
{
    public partial class AddSupplyRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplyRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    RequestDate = table.Column<DateTime>(
                        type: "datetime2", nullable: false),

                    TotalPrice = table.Column<decimal>(
                        type: "decimal(18,2)", nullable: false),

                    Status = table.Column<string>(
                        type: "nvarchar(max)", nullable: false),

                    UserId = table.Column<string>(
                        type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyRequests", x => x.Id);

                    table.ForeignKey(
                        name: "FK_SupplyRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    SupplyRequestId = table.Column<int>(
                        type: "int", nullable: false),

                    ProductId = table.Column<int>(
                        type: "int", nullable: false),

                    Quantity = table.Column<int>(
                        type: "int", nullable: false),

                    Price = table.Column<decimal>(
                        type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestItems", x => x.Id);

                    table.ForeignKey(
                        name: "FK_RequestItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_RequestItems_SupplyRequests_SupplyRequestId",
                        column: x => x.SupplyRequestId,
                        principalTable: "SupplyRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestItems_ProductId",
                table: "RequestItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestItems_SupplyRequestId",
                table: "RequestItems",
                column: "SupplyRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyRequests_UserId",
                table: "SupplyRequests",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestItems");

            migrationBuilder.DropTable(
                name: "SupplyRequests");
        }
    }
}
