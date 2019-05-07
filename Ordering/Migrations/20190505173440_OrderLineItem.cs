using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.Migrations
{
    public partial class OrderLineItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_line_item_product_product_id",
                table: "order_line_item");

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "order_line_item",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_order_line_item_product_product_id",
                table: "order_line_item",
                column: "product_id",
                principalTable: "product",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_line_item_product_product_id",
                table: "order_line_item");

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "order_line_item",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_order_line_item_product_product_id",
                table: "order_line_item",
                column: "product_id",
                principalTable: "product",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
