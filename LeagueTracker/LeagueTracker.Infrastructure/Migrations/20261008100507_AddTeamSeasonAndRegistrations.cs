using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamSeasonAndRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamSeasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    SeasonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamSeasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamSeasons_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamSeasons_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    TeamSeasonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerRegistrations_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerRegistrations_TeamSeasons_TeamSeasonId",
                        column: x => x.TeamSeasonId,
                        principalTable: "TeamSeasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRegistrations_PlayerId_TeamSeasonId",
                table: "PlayerRegistrations",
                columns: new[] { "PlayerId", "TeamSeasonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRegistrations_TeamSeasonId",
                table: "PlayerRegistrations",
                column: "TeamSeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamSeasons_SeasonId",
                table: "TeamSeasons",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamSeasons_TeamId_SeasonId",
                table: "TeamSeasons",
                columns: new[] { "TeamId", "SeasonId" },
                unique: true);

            migrationBuilder.Sql(@"
                INSERT INTO TeamSeasons (TeamId, SeasonId)
                SELECT Id, SeasonId FROM Teams;

                INSERT INTO PlayerRegistrations (PlayerId, TeamSeasonId)
                SELECT p.Id, ts.Id
                FROM Players p
                JOIN TeamSeasons ts ON ts.TeamId = p.TeamId;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerRegistrations");

            migrationBuilder.DropTable(
                name: "TeamSeasons");
        }
    }
}
