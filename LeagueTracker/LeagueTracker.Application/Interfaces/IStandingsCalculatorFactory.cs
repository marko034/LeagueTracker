

namespace LeagueTracker.Application.Interfaces;

public interface IStandingsCalculatorFactory
{
    IStandingsCalculator GetCalculator(string sportName);
}
