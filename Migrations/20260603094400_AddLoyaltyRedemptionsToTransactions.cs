using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankR.Migrations
{
    /// <inheritdoc />
    public partial class AddLoyaltyRedemptionsToTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LoyaltyDiscountMkd",
                table: "Transactions",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PointsRedeemed",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoyaltyDiscountMkd",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PointsRedeemed",
                table: "Transactions");
        }
    }
}
