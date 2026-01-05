using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingBookingFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserDetails_LoginId",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "LoginId",
                table: "UserDetails");

            migrationBuilder.RenameColumn(
                name: "NumberOfTickets",
                table: "Tickets",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserId",
                table: "Tickets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_UserDetails_UserId",
                table: "Tickets",
                column: "UserId",
                principalTable: "UserDetails",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_UserDetails_UserId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_UserId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Tickets",
                newName: "NumberOfTickets");

            migrationBuilder.AddColumn<string>(
                name: "LoginId",
                table: "UserDetails",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_LoginId",
                table: "UserDetails",
                column: "LoginId",
                unique: true);
        }
    }
}
