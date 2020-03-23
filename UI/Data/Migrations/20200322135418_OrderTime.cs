using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UI.Data.Migrations
{
    public partial class OrderTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderHeader",
                columns: table => new
                {
                    OrderHeaderID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(nullable: false),
                    OrderTotal = table.Column<double>(nullable: false),
                    PickUpTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    PaymentStatus = table.Column<string>(nullable: true),
                    PickUpName = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    TransactionID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHeader", x => x.OrderHeaderID);
                    table.ForeignKey(
                        name: "FK_OrderHeader_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailsID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(nullable: false),
                    AnnounceID = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailsID);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Announce_AnnounceID",
                        column: x => x.AnnounceID,
                        principalTable: "Announce",
                        principalColumn: "AnnounceID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_OrderHeader_OrderID",
                        column: x => x.OrderID,
                        principalTable: "OrderHeader",
                        principalColumn: "OrderHeaderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_AnnounceID",
                table: "OrderDetails",
                column: "AnnounceID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderID",
                table: "OrderDetails",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_UserID",
                table: "OrderHeader",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "OrderHeader");
        }
    }
}
