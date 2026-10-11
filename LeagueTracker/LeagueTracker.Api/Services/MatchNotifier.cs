using LeagueTracker.Api.Hubs;
using LeagueTracker.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace LeagueTracker.Api.Services;

public class MatchNotifier : IMatchNotifier
{
    private readonly IHubContext<MatchHub> _hubContext;

    public MatchNotifier(IHubContext<MatchHub> hubContext)
    {
        _hubContext = hubContext;
    }
    public async Task NotifyMatchUpdatedAsync(int matchId, int seasonId)
    {
        await _hubContext.Clients.Group($"season-{seasonId}").SendAsync("Match updated", new { matchId, seasonId });
    }
}
