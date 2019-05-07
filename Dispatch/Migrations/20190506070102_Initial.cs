using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Dispatch.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dispatch_order",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    order_id = table.Column<int>(nullable: false),
                    delivery_address = table.Column<string>(nullable: true),
                    disptach_status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispatch_order", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "delivery",
                columns: table => new
                {
                    dispatch_order_id = table.Column<int>(nullable: false),
                    invoice_id = table.Column<int>(nullable: false),
                    invoice_amount = table.Column<double>(nullable: false),
                    freight_forwarder_id = table.Column<string>(nullable: true),
                    delivery_address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delivery", x => x.dispatch_order_id);
                    table.ForeignKey(
                        name: "FK_delivery_dispatch_order_dispatch_order_id",
                        column: x => x.dispatch_order_id,
                        principalTable: "dispatch_order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delivery");

            migrationBuilder.DropTable(
                name: "dispatch_order");
        }
    }
}
