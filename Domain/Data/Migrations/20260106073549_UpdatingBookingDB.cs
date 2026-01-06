using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingBookingDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoginID",
                table: "UserDetails",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_LoginID",
                table: "UserDetails",
                column: "LoginID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserDetails_LoginID",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "LoginID",
                table: "UserDetails");
        }
    }
}
