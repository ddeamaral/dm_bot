using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dm_bot.Migrations
{
    public partial class PlayerItemAndPlayDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PlayDate",
                table: "DungeonMasterAvailabilities",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    GoldCost = table.Column<float>(nullable: false),
                    SellValue = table.Column<float>(nullable: false)
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
                    DiscordMention = table.Column<string>(nullable: true),
                    Gold = table.Column<float>(nullable: false),
                    Silver = table.Column<float>(nullable: false),
                    Copper = table.Column<float>(nullable: false),
                    Platinum = table.Column<float>(nullable: false),
                    RoleIds = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropColumn(
                name: "PlayDate",
                table: "DungeonMasterAvailabilities");
        }
    }
}
