
using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(LeagueTrackerDbContext context)
    {
        if (await context.Seasons.AnyAsync())
        {
            return;
        }

        var sport = new Sport { Name = "Stoni tenis" };
        var league = new League { Sport =  sport, Name = "Prva Liga"};

        var season = new Season
        {
            League = league,
            Name = "2025/26",
            StartDate = new DateTime(2025, 9, 1)
        };

        context.Seasons.Add(season);
        await context.SaveChangesAsync();
     }

    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "Competitor" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        var adminEmail = "marko.markoilic@gmail.com";

        if(await userManager.FindByEmailAsync(adminEmail) is not null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            DisplayName = "Marko admin"
        };

        await userManager.CreateAsync(admin, "ChangeMe123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}
