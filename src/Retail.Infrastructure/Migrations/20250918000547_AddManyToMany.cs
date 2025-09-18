using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retail.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_QuarterlyPlans_QuarterlyPlanId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_QuarterlyPlanId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "QuarterlyPlanId",
                table: "Suppliers");

            migrationBuilder.CreateTable(
                name: "QuarterlyPlanSupplier",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "integer", nullable: false),
                    QuarterlyPlanId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuarterlyPlanSupplier", x => new { x.SupplierId, x.QuarterlyPlanId });
                    table.ForeignKey(
                        name: "FK_QuarterlyPlanSupplier_QuarterlyPlans_QuarterlyPlanId",
                        column: x => x.QuarterlyPlanId,
                        principalTable: "QuarterlyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuarterlyPlanSupplier_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuarterlyPlanSupplier_QuarterlyPlanId",
                table: "QuarterlyPlanSupplier",
                column: "QuarterlyPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuarterlyPlanSupplier");

            migrationBuilder.AddColumn<int>(
                name: "QuarterlyPlanId",
                table: "Suppliers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_QuarterlyPlanId",
                table: "Suppliers",
                column: "QuarterlyPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_QuarterlyPlans_QuarterlyPlanId",
                table: "Suppliers",
                column: "QuarterlyPlanId",
                principalTable: "QuarterlyPlans",
                principalColumn: "Id");
        }
    }
}
