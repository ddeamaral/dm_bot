using System;
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
                    CombatPercent = table.Column<int>(nullable: false),
                    PlayDate = table.Column<DateTime>(nullable: false)
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
                        .Annotation("Sqlite:Autoincrement", true),
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
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterName = table.Column<string>(nullable: true),
                    DiscordMention = table.Column<string>(nullable: true),
                    Gold = table.Column<decimal>(nullable: false),
                    Silver = table.Column<decimal>(nullable: false),
                    Copper = table.Column<decimal>(nullable: false),
                    Electrum = table.Column<decimal>(nullable: false),
                    RoleIds = table.Column<string>(nullable: true)
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
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    JobLink = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
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

            migrationBuilder.CreateTable(
                name: "InventoryItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Quantity = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: true),
                    PlayerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItem", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InventoryItem_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryItem_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_ItemId",
                table: "InventoryItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_PlayerId",
                table: "InventoryItem",
                column: "PlayerId");

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
                name: "InventoryItem");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Ranks");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "DungeonMasterAvailabilities");
        }
    }
}
