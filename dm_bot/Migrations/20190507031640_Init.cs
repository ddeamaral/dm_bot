using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    DungeonMasterUserName = table.Column<string>(nullable: true),
                    ChronusTimeLink = table.Column<string>(nullable: true),
                    ChatCommChannel = table.Column<string>(nullable: true),
                    VoiceCommChannel = table.Column<string>(nullable: true),
                    MinHours = table.Column<int>(nullable: false),
                    MaxHours = table.Column<int>(nullable: false),
                    RoleplayingPercent = table.Column<int>(nullable: false),
                    CombatPercent = table.Column<int>(nullable: false),
                    Roll20Link = table.Column<string>(nullable: true),
                    PlayDate = table.Column<DateTime>(nullable: false),
                    PlayDateOffset = table.Column<DateTimeOffset>(nullable: false),
                    MiscellaneousText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DungeonMasterAvailabilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    GoldCost = table.Column<decimal>(nullable: false),
                    CopperCost = table.Column<decimal>(nullable: false),
                    SilverCost = table.Column<decimal>(nullable: false),
                    ElectrumCost = table.Column<decimal>(nullable: false),
                    IsTradeOnly = table.Column<bool>(nullable: false),
                    SellValue = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CharacterName = table.Column<string>(nullable: true),
                    DiscordMention = table.Column<string>(nullable: true),
                    Gold = table.Column<decimal>(nullable: false),
                    Silver = table.Column<decimal>(nullable: false),
                    Copper = table.Column<decimal>(nullable: false),
                    Electrum = table.Column<decimal>(nullable: false),
                    Pips = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    JobLink = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    Difficulty = table.Column<int>(nullable: false),
                    FirstApproval = table.Column<string>(nullable: true),
                    SecondApproval = table.Column<string>(nullable: true),
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
                name: "Lobbies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AvailabilityId = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lobbies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lobbies_DungeonMasterAvailabilities_AvailabilityId",
                        column: x => x.AvailabilityId,
                        principalTable: "DungeonMasterAvailabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lobbies_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RankName = table.Column<string>(nullable: true),
                    RankLetter = table.Column<string>(nullable: true),
                    RankMention = table.Column<string>(nullable: true),
                    JobId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ranks_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_DungeonMasterAvailabilityId",
                table: "Jobs",
                column: "DungeonMasterAvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Lobbies_AvailabilityId",
                table: "Lobbies",
                column: "AvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Lobbies_PlayerId",
                table: "Lobbies",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_JobId",
                table: "Ranks",
                column: "JobId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Lobbies");

            migrationBuilder.DropTable(
                name: "Ranks");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "DungeonMasterAvailabilities");
        }
    }
}
