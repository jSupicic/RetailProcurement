using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Retail.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeStatisticsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuarterlyPlanSuppliers");

            migrationBuilder.AddColumn<int[]>(
                name: "SupplierIds",
                table: "QuarterlyPlans",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierIds",
                table: "QuarterlyPlans");

            migrationBuilder.CreateTable(
                name: "QuarterlyPlanSuppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuarterlyPlanSuppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuarterlyPlanSuppliers_QuarterlyPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "QuarterlyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuarterlyPlanSuppliers_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuarterlyPlanSuppliers_PlanId",
                table: "QuarterlyPlanSuppliers",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_QuarterlyPlanSuppliers_SupplierId",
                table: "QuarterlyPlanSuppliers",
                column: "SupplierId");
        }
    }
}
