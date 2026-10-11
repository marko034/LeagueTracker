

using LeagueTracker.Application.DTOs.Games;
using LeagueTracker.Application.Exceptions;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Data;
using LeagueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Infrastructure.Services;

public class GameService : IGameService
{
    private readonly LeagueTrackerDbContext _context;
    private readonly IMatchNotifier _matchNotifier;
    private readonly UserManager<ApplicationUser> _userManager;


    public GameService(LeagueTrackerDbContext context, IMatchNotifier matchNotifier, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _matchNotifier = matchNotifier;
        _userManager = userManager;
    }

    public async Task EnsureCanRecordAsync(int homePlayer1Id, int? homePlayer2Id, int awayPlayer1Id, int? awayPlayer2Id, string userId, bool isAdmin)
    {
        if (isAdmin)
        {
            return; // admin sme sve
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user?.PlayerId is null)
        {
            throw new ForbiddenOperationException("Nalog nije povezan ni sa jednim igračem.");
        }

        var involvedPlayerIds = new[] { homePlayer1Id, homePlayer2Id, awayPlayer1Id, awayPlayer2Id };
        if (!involvedPlayerIds.Contains(user.PlayerId.Value))
        {
            throw new ForbiddenOperationException("Ne možete unositi rezultat partije u kojoj ne igrate.");
        }
    }
    public async Task<FinalizeMatchResultDto> FinalizeMatchAsync(int matchId, string userId, bool isAdmin)
    {
        var match = await _context.Matches
            .Include(m => m.Games)
            .ThenInclude(g => g.Sets)
            .FirstOrDefaultAsync(m => m.Id == matchId);

        if(match is null)
        {
            return null;
        }

        if (!isAdmin)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user?.PlayerId is null)
            {
                throw new ForbiddenOperationException("Nalog nije povezan ni sa jednim igračem.");
            }

            var playsInMatch = match.Games.Any(g =>
                g.HomePlayer1Id == user.PlayerId || g.HomePlayer2Id == user.PlayerId ||
                g.AwayPlayer1Id == user.PlayerId || g.AwayPlayer2Id == user.PlayerId);

            if (!playsInMatch)
            {
                throw new ForbiddenOperationException("Ne možete finalizovati meč u kom ne igrate.");
            }
        }

        int homeGamesWon = 0;
        int awayGamesWon = 0;

        foreach(var game in match.Games)
        {
            var homeSetsWon = game.Sets.Count(s => s.HomeScore > s.AwayScore);
            var awaySetsWon = game.Sets.Count(s => s.HomeScore < s.AwayScore);

            if(homeSetsWon > awaySetsWon)
            {
                homeGamesWon++;
            }
            else if(awaySetsWon > homeSetsWon)
            {
                awayGamesWon++;
            }
        }

        match.HomeScore = homeGamesWon;
        match.AwayScore = awayGamesWon;
        match.Status = MatchStatus.AwaitingVerification;

        await _context.SaveChangesAsync();

        await _matchNotifier.NotifyMatchUpdatedAsync(match.Id, match.SeasonId);

        return new FinalizeMatchResultDto
        {
            MatchId = match.Id,
            HomeScore = match.HomeScore!.Value,
            AwayScore = match.AwayScore!.Value,
            Status = match.Status.ToString()
        };
    }

    public async Task<GameDto> RecordGameAsync(CreateGameDto dto, string userId, bool isAdmin)
    {
        await EnsureCanRecordAsync(dto.HomePlayer1Id, dto.HomePlayer2Id, dto.AwayPlayer1Id, dto.AwayPlayer2Id, userId, isAdmin);

        var game = new Game
        {
            MatchId = dto.MatchId,
            Type = dto.Type,
            HomePlayer1Id = dto.HomePlayer1Id,
            HomePlayer2Id = dto.HomePlayer2Id,
            AwayPlayer1Id = dto.AwayPlayer1Id,
            AwayPlayer2Id = dto.AwayPlayer2Id,
            Sets = dto.Sets.Select(s => new GameSet
            {
                SetNumber = s.SetNumber,
                HomeScore = s.HomeScore,
                AwayScore = s.AwayScore
            }).ToList()
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();
        return MapToDto(game);
    }

    private static GameDto MapToDto(Game game)
    {
        return new GameDto
        {
            Id = game.Id,
            HomePlayer1Id = game.HomePlayer1Id,
            HomePlayer2Id = game.HomePlayer2Id,
            AwayPlayer1Id = game.AwayPlayer1Id,
            AwayPlayer2Id = game.AwayPlayer2Id,
            HomeWon = game.Sets.Count(s => s.HomeScore > s.AwayScore) > game.Sets.Count(s => s.AwayScore > s.HomeScore),
            Sets = game.Sets.Select(s => new CreateGameSetDto
            {
                SetNumber = s.SetNumber,
                HomeScore = s.HomeScore,
                AwayScore = s.AwayScore
            }).ToList()
        };
    }

    public async Task<bool> VerifyMatchAsync(int matchId)
    {
        var match = await _context.Matches.FindAsync(matchId);
        if(match is null || match.Status != MatchStatus.AwaitingVerification)
        {
            return false;
        }

        match.Status = MatchStatus.Verified;
        await _context.SaveChangesAsync();

        await _matchNotifier.NotifyMatchUpdatedAsync(match.Id, match.SeasonId);

        return true;
    }
}
