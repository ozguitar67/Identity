using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_App.Migrations.ApplicationIdentityDb
{
    /// <inheritdoc />
    public partial class AddCustomUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Department",
                schema: "user",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                schema: "user",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                schema: "user",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Position",
                schema: "user",
                table: "AspNetUsers");
        }
    }
}
