using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TankR.Migrations
{
    /// <inheritdoc />
    public partial class ImprovedAddressSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stations_Addresses_AddressId",
                table: "Stations");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Addresses_AddressId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Users_AddressId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Stations_AddressId",
                table: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_StationEmployees_StationId",
                table: "StationEmployees");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Stations");

            migrationBuilder.CreateTable(
                name: "StationAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StationId = table.Column<int>(type: "integer", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: true),
                    StreetNumber = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<int>(type: "integer", nullable: false),
                    Lat = table.Column<decimal>(type: "numeric", nullable: false),
                    Lng = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StationAddresses_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: true),
                    StreetNumber = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<int>(type: "integer", nullable: false),
                    Lat = table.Column<decimal>(type: "numeric", nullable: false),
                    Lng = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAddresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StationEmployees_StationId_UserId",
                table: "StationEmployees",
                columns: new[] { "StationId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StationAddresses_StationId",
                table: "StationAddresses",
                column: "StationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                table: "UserAddresses",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StationAddresses");

            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropIndex(
                name: "IX_StationEmployees_StationId_UserId",
                table: "StationEmployees");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Stations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    City = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<int>(type: "integer", nullable: false),
                    Lat = table.Column<decimal>(type: "numeric", nullable: false),
                    Lng = table.Column<decimal>(type: "numeric", nullable: false),
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressId",
                table: "Users",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_AddressId",
                table: "Stations",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_StationEmployees_StationId",
                table: "StationEmployees",
                column: "StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stations_Addresses_AddressId",
                table: "Stations",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Addresses_AddressId",
                table: "Users",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id");
        }
    }
}
