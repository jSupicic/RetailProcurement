using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Retail.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuarterlyPlanSuppliers",
                table: "QuarterlyPlanSuppliers");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "QuarterlyPlanSuppliers",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuarterlyPlanSuppliers",
                table: "QuarterlyPlanSuppliers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_QuarterlyPlanSuppliers_PlanId",
                table: "QuarterlyPlanSuppliers",
                column: "PlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuarterlyPlanSuppliers",
                table: "QuarterlyPlanSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_QuarterlyPlanSuppliers_PlanId",
                table: "QuarterlyPlanSuppliers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "QuarterlyPlanSuppliers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuarterlyPlanSuppliers",
                table: "QuarterlyPlanSuppliers",
                columns: new[] { "PlanId", "SupplierId" });
        }
    }
}
