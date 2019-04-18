﻿using System;
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RankName = table.Column<string>(nullable: true),
                    RankLetter = table.Column<string>(nullable: true),
                    DungeonMasterAvailabilityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ranks_DungeonMasterAvailabilities_DungeonMasterAvailability~",
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
                name: "Items");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Ranks");

            migrationBuilder.DropTable(
                name: "DungeonMasterAvailabilities");
        }
    }
}