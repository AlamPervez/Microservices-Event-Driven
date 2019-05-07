using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.Migrations
{
    public partial class AddDispatchIdColumnInOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "dispatch_id",
                table: "order",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dispatch_id",
                table: "order");
        }
    }
}
