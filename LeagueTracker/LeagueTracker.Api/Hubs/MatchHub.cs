using Microsoft.AspNetCore.SignalR;

namespace LeagueTracker.Api.Hubs;

public class MatchHub:Hub
{
    public async Task JoinSeasonGroup(int seasonId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(seasonId));
    }

    public async Task LeaveSeasonGroup(int seasonId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(seasonId));
    }

    public static string GroupName(int seasonId) => $"season-{seasonId}";
}
