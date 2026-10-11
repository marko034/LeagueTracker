using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGameAndGameSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    HomePlayer1Id = table.Column<int>(type: "int", nullable: false),
                    HomePlayer2Id = table.Column<int>(type: "int", nullable: true),
                    AwayPlayer1Id = table.Column<int>(type: "int", nullable: false),
                    AwayPlayer2Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Players_AwayPlayer1Id",
                        column: x => x.AwayPlayer1Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Players_AwayPlayer2Id",
                        column: x => x.AwayPlayer2Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Players_HomePlayer1Id",
                        column: x => x.HomePlayer1Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Players_HomePlayer2Id",
                        column: x => x.HomePlayer2Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SetNumber = table.Column<int>(type: "int", nullable: false),
                    HomeScore = table.Column<int>(type: "int", nullable: false),
                    AwayScore = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSets_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_AwayPlayer1Id",
                table: "Games",
                column: "AwayPlayer1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_AwayPlayer2Id",
                table: "Games",
                column: "AwayPlayer2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_HomePlayer1Id",
                table: "Games",
                column: "HomePlayer1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_HomePlayer2Id",
                table: "Games",
                column: "HomePlayer2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_MatchId",
                table: "Games",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSets_GameId",
                table: "GameSets",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSets");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
