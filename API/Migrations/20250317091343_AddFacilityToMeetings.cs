using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddFacilityToMeetings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Meetings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MeetingId",
                table: "Facilities",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_MeetingId",
                table: "Facilities",
                column: "MeetingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Meetings_MeetingId",
                table: "Facilities",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Meetings_MeetingId",
                table: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_MeetingId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "MeetingId",
                table: "Facilities");
        }
    }
}
