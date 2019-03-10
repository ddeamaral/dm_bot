using Microsoft.EntityFrameworkCore.Migrations;

namespace dm_bot.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DungeonMasterAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DungeonMasterUserName = table.Column<string>(nullable: true),
                    ChronusTimeLink = table.Column<string>(nullable: true),
                    ChatCommChannel = table.Column<string>(nullable: true),
                    VoiceCommChannel = table.Column<string>(nullable: true),
                    MinHours = table.Column<int>(nullable: false),
                    MaxHours = table.Column<int>(nullable: false),
                    RoleplayingPercent = table.Column<int>(nullable: false),
                    CombatPercent = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DungeonMasterAvailabilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Difficulty = table.Column<int>(nullable: false),
                    DungeonMasterAvailabilityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_DungeonMasterAvailabilities_DungeonMasterAvailabilityId",
                        column: x => x.DungeonMasterAvailabilityId,
                        principalTable: "DungeonMasterAvailabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RankName = table.Column<string>(nullable: true),
                    RankLetter = table.Column<string>(nullable: true),
                    DungeonMasterAvailabilityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ranks_DungeonMasterAvailabilities_DungeonMasterAvailabilityId",
                        column: x => x.DungeonMasterAvailabilityId,
                        principalTable: "DungeonMasterAvailabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_DungeonMasterAvailabilityId",
                table: "Jobs",
                column: "DungeonMasterAvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_DungeonMasterAvailabilityId",
                table: "Ranks",
                column: "DungeonMasterAvailabilityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Ranks");

            migrationBuilder.DropTable(
                name: "DungeonMasterAvailabilities");
        }
    }
}
