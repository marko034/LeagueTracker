

using LeagueTracker.Application.DTOs.Players;
using LeagueTracker.Application.Exceptions;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Infrastructure.Services;

public class PlayerService : IPlayerService
{
    private readonly LeagueTrackerDbContext _context;

    public PlayerService(LeagueTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlayerDto>> GetAsync(int? teamId, int? seasonId)
    {
        var hasTeam = teamId.HasValue;
        var hasSeason = seasonId.HasValue;
        var tid = teamId.GetValueOrDefault();
        var sid = seasonId.GetValueOrDefault();

        return await _context.Players
            .AsNoTracking()
            .Where(p => (!hasTeam && !hasSeason) || p.Registrations.Any(r =>
            (!hasTeam || r.TeamSeason.TeamId == tid) &&
            (!hasSeason || r.TeamSeason.SeasonId == sid)))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(p => new PlayerDto { Id = p.Id, 
                FirstName = p.FirstName, 
                LastName = p.LastName, 
                PhotoUrl = p.PhotoUrl, 
                DateOfBirth = p.DateOfBirth,
                TeamId = p.Registrations.Where(r => hasSeason && r.TeamSeason.SeasonId == sid)
                .Select(r => (int?)r.TeamSeason.TeamId).FirstOrDefault(),
                TeamName = p.Registrations
                .Where(r => hasSeason && r.TeamSeason.SeasonId == sid)
                .Select(r => r.TeamSeason.Team.Name).FirstOrDefault()
            })
            .ToListAsync();
    }
    public async Task<PlayerDto?> GetByIdAsync(int id, int? seasonId)
    {
        var hasSeason = seasonId.HasValue;
        var sid = seasonId.GetValueOrDefault();

        return await _context.Players
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PlayerDto 
            {
                Id = p.Id, 
                FirstName = p.FirstName, 
                LastName = p.LastName, 
                PhotoUrl = p.PhotoUrl, 
                DateOfBirth = p.DateOfBirth,
                TeamId = p.Registrations
                .Where(r => hasSeason && r.TeamSeason.SeasonId == sid)
                .Select(r => (int?)r.TeamSeason.TeamId).FirstOrDefault(),
                TeamName = p.Registrations
                .Where(r => hasSeason && r.TeamSeason.SeasonId == sid)
                .Select(r => r.TeamSeason.Team.Name).FirstOrDefault()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PlayerDto> CreateAsync(CreatePlayerDto dto)
    {
        var player = new Player
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhotoUrl = dto.PhotoUrl,
            DateOfBirth = dto.DateOfBirth
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return ToDto(player);
    }
    public async Task<PlayerDto?> UpdateAsync(int id, CreatePlayerDto dto)
    {
        var player = await _context.Players.FindAsync(id);
        if (player is null) return null;

        player.FirstName = dto.FirstName;
        player.LastName = dto.LastName;
        player.PhotoUrl = dto.PhotoUrl;
        player.DateOfBirth = dto.DateOfBirth;

        await _context.SaveChangesAsync();

        return ToDto(player);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var player = await _context.Players.FindAsync(id);
        if(player is null)
        {
            return false;
        }

        var hasGames = await _context.Games.AnyAsync(g => 
            g.HomePlayer1Id == id || g.HomePlayer2Id == id ||
            g.AwayPlayer1Id == id || g.AwayPlayer2Id ==id);

        if (hasGames || await _context.PlayerRegistrations.AnyAsync(r => r.PlayerId == id))
        {
            throw new BusinessRuleException("Igrač ima prijave u sezonama ili odigrane partije i ne može biti obrisan.");
        }

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<bool> RegisterAsync(int playerId, RegisterPlayerDto dto)
    {
        if(!await _context.Players.AnyAsync(p => p.Id == playerId))
        {
            return false;
        }

        var teamSeason = await _context.TeamSeasons
            .FirstOrDefaultAsync(ts => ts.TeamId == dto.TeamId && ts.SeasonId == dto.SeasonId)
            ?? throw new BusinessRuleException("Tim nije prijavljen na izabranu sezonu.");

        var alreadyInSeason = await _context.PlayerRegistrations
            .AnyAsync(pr => pr.Id == playerId && pr.TeamSeason.SeasonId == dto.SeasonId);

        if (alreadyInSeason)
        {
            throw new BusinessRuleException("Igrač je već prijavljen u ovoj sezoni.");
        }

        _context.PlayerRegistrations.Add(new PlayerRegistration { PlayerId = playerId, TeamSeasonId = teamSeason.Id });
        await _context.SaveChangesAsync(); 
        return true;
    }

    public async Task<bool> UnregisterAsync(int playerId, int seasonId)
    {
        var registration = await _context.PlayerRegistrations
            .FirstOrDefaultAsync(r => r.PlayerId == playerId && r.TeamSeason.SeasonId ==  seasonId);

        if (registration is null) return false;

        _context.PlayerRegistrations.Remove(registration);
        await _context.SaveChangesAsync();
        return true;
    }


    private static PlayerDto ToDto(Player p) => new()
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        PhotoUrl = p.PhotoUrl,
        DateOfBirth = p.DateOfBirth
    };
}
