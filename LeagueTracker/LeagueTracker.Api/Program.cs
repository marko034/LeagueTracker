using LeagueTracker.Api.Hubs;
using LeagueTracker.Api.Middleware;
using LeagueTracker.Api.Services;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Infrastructure.Data;
using LeagueTracker.Infrastructure.Identity;
using LeagueTracker.Infrastructure.Services;
using LeagueTracker.Infrastructure.Services.Standings;
using LeagueTracker.Infrastructure.Services.Stats;
using LeagueTracker.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<LeagueTrackerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<LeagueTrackerDbContext>()
.AddDefaultTokenProviders();

builder.Services
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer();

builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtSettings>>((bearer, jwtOptions) =>
    {
        var jwt = jwtOptions.Value;

        bearer.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
        };
    });

builder.Services.AddSignalR();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<ISportService,SportService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

builder.Services.AddScoped<IStandingsCalculator, TableTennisStandingsCalculator>();
builder.Services.AddScoped<IStandingsCalculatorFactory, StandingsCalculatorFactory>();
builder.Services.AddScoped<IStandingsService, StandingsService>();

builder.Services.AddScoped<IPlayerStatsCalculator, TableTennisPlayerStatsCalculator>();
builder.Services.AddScoped<IPlayerStatsCalculatorFactory, PlayerStatsCalculatorFactory>();
builder.Services.AddScoped<IPlayerStatsService, PlayerStatsService>();

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IMatchService, MatchService>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IMatchNotifier, MatchNotifier>();

builder.Services.AddScoped<ISeasonService, SeasonService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();

builder.Services.AddScoped<ILeagueService, LeagueService>();

builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();
builder.Services.AddHostedService<OrphanedUploadsCleaner>();
builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LeagueTrackerDbContext>();
    await DbSeeder.SeedAsync(context);

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await DbSeeder.SeedRolesAsync(roleManager);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await DbSeeder.SeedAdminUserAsync(userManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        if (context.Context.Request.Path.StartsWithSegments("/uploads"))
        {
            context.Context.Response.Headers.CacheControl = "public,max-age=31536000,immutable";
        }
    }
});

app.MapControllers();

app.MapHub<MatchHub>("hubs/matches");

app.Run();
