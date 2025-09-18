using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retail.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeyCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierIds",
                table: "QuarterlyPlans");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int[]>(
                name: "SupplierIds",
                table: "QuarterlyPlans",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }
    }
}
