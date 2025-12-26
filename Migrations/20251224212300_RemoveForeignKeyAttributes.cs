using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankR.Migrations
{
    /// <inheritdoc />
    public partial class RemoveForeignKeyAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_CustomerId",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_CustomerId",
                table: "Transactions",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_CustomerId",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Transactions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_CustomerId",
                table: "Transactions",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
