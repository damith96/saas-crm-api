using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCRM.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCompanyConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserCompanies_UserId",
                table: "UserCompanies");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "UserCompanies",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_AssignedByUserId",
                table: "UserCompanies",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_UserId_CompanyId",
                table: "UserCompanies",
                columns: new[] { "UserId", "CompanyId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompanies_Users_AssignedByUserId",
                table: "UserCompanies",
                column: "AssignedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCompanies_Users_AssignedByUserId",
                table: "UserCompanies");

            migrationBuilder.DropIndex(
                name: "IX_UserCompanies_AssignedByUserId",
                table: "UserCompanies");

            migrationBuilder.DropIndex(
                name: "IX_UserCompanies_UserId_CompanyId",
                table: "UserCompanies");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "UserCompanies",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_UserId",
                table: "UserCompanies",
                column: "UserId");
        }
    }
}
