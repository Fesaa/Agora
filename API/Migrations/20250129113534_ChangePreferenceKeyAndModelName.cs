using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class ChangePreferenceKeyAndModelName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppUserPreferences",
                table: "AppUserPreferences");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AppUserPreferences");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppUserPreferences",
                table: "AppUserPreferences",
                column: "ExternalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppUserPreferences",
                table: "AppUserPreferences");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AppUserPreferences",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppUserPreferences",
                table: "AppUserPreferences",
                column: "Id");
        }
    }
}
