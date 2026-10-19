using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeMediaPaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE Teams
                SET LogoUrl = SUBSTRING(LogoUrl, CHARINDEX('/uploads/', LogoUrl), LEN(LogoUrl))
                WHERE LogoUrl LIKE 'http%/uploads/%';

                UPDATE Players
                SET PhotoUrl = SUBSTRING(PhotoUrl, CHARINDEX('/uploads/', PhotoUrl), LEN(PhotoUrl))
                WHERE PhotoUrl LIKE 'http%/uploads/%';
             "); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
