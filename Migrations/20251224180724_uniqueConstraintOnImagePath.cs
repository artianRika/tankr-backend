using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankR.Migrations
{
    /// <inheritdoc />
    public partial class uniqueConstraintOnImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StationPhotos_ImagePath",
                table: "StationPhotos",
                column: "ImagePath",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StationPhotos_ImagePath",
                table: "StationPhotos");
        }
    }
}
