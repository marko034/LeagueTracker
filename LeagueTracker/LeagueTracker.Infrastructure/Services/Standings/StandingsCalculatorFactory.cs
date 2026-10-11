using LeagueTracker.Application.Interfaces;

namespace LeagueTracker.Infrastructure.Services.Standings;

public class StandingsCalculatorFactory : IStandingsCalculatorFactory
{
    private readonly IEnumerable<IStandingsCalculator> _calculators;

    public StandingsCalculatorFactory(IEnumerable<IStandingsCalculator> calculators)
    {
        _calculators = calculators;
    }
    public IStandingsCalculator GetCalculator(string sportName)
    {
        var calculator = _calculators.FirstOrDefault(c=>c.SportName == sportName);

        if (calculator is null)
        {
            throw new NotSupportedException($"Nema kalkulatora za sport: {sportName}");
        }

        return calculator;
    }
}
