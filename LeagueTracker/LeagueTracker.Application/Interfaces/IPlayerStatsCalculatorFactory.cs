namespace LeagueTracker.Application.Interfaces;

public interface IPlayerStatsCalculatorFactory
{
    IPlayerStatsCalculator GetCalculator(string sportName);
}
