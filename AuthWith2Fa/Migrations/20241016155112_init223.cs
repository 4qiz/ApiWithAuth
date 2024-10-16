using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthWith2Fa.Migrations
{
    /// <inheritdoc />
    public partial class init223 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "7ad58e2e-6c35-4d4a-acd8-60f6c1b7vbcb", "7ad58e2e-6c35-4d4a-acd8-60f6c1b75bcb" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "7ad58e2e-6c35-4d4a-acd8-60f6c1b7vbcb", "7ad58e2e-6c35-4d4a-acd8-60f6c1b75bcb" });
        }
    }
}
