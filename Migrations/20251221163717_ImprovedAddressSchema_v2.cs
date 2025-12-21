using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankR.Migrations
{
    /// <inheritdoc />
    public partial class ImprovedAddressSchema_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Lng",
                table: "UserAddresses",
                type: "numeric(10,7)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "UserAddresses",
                type: "numeric(10,7)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lng",
                table: "StationAddresses",
                type: "numeric(10,7)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "StationAddresses",
                type: "numeric(10,7)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Lng",
                table: "UserAddresses",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,7)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "UserAddresses",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,7)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lng",
                table: "StationAddresses",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,7)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "StationAddresses",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,7)");
        }
    }
}
