using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class FW111CreateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    AlertManagement = table.Column<bool>(type: "INTEGER", nullable: false),
                    Cost = table.Column<float>(type: "REAL", nullable: false),
                    DayRange = table.Column<string>(type: "TEXT", nullable: false),
                    TimeRange = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeetingRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    MayExceedCapacity = table.Column<bool>(type: "INTEGER", nullable: false),
                    MergeAble = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacilityMeetingRoom",
                columns: table => new
                {
                    FacilitiesId = table.Column<int>(type: "INTEGER", nullable: false),
                    MeetingRoomsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityMeetingRoom", x => new { x.FacilitiesId, x.MeetingRoomsId });
                    table.ForeignKey(
                        name: "FK_FacilityMeetingRoom_Facilities_FacilitiesId",
                        column: x => x.FacilitiesId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacilityMeetingRoom_MeetingRooms_MeetingRoomsId",
                        column: x => x.MeetingRoomsId,
                        principalTable: "MeetingRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MergeRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentRoomId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MergeRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MergeRooms_MeetingRooms_ParentRoomId",
                        column: x => x.ParentRoomId,
                        principalTable: "MeetingRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeetingRoomMergeRooms",
                columns: table => new
                {
                    MeetingRoomsId = table.Column<int>(type: "INTEGER", nullable: false),
                    MergeRoomsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingRoomMergeRooms", x => new { x.MeetingRoomsId, x.MergeRoomsId });
                    table.ForeignKey(
                        name: "FK_MeetingRoomMergeRooms_MeetingRooms_MeetingRoomsId",
                        column: x => x.MeetingRoomsId,
                        principalTable: "MeetingRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingRoomMergeRooms_MergeRooms_MergeRoomsId",
                        column: x => x.MergeRoomsId,
                        principalTable: "MergeRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacilityMeetingRoom_MeetingRoomsId",
                table: "FacilityMeetingRoom",
                column: "MeetingRoomsId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomMergeRooms_MergeRoomsId",
                table: "MeetingRoomMergeRooms",
                column: "MergeRoomsId");

            migrationBuilder.CreateIndex(
                name: "IX_MergeRooms_ParentRoomId",
                table: "MergeRooms",
                column: "ParentRoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityMeetingRoom");

            migrationBuilder.DropTable(
                name: "MeetingRoomMergeRooms");

            migrationBuilder.DropTable(
                name: "Facilities");

            migrationBuilder.DropTable(
                name: "MergeRooms");

            migrationBuilder.DropTable(
                name: "MeetingRooms");
        }
    }
}
