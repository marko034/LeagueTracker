
using LeagueTracker.Application.Interfaces;

namespace LeagueTracker.Infrastructure.Services.Stats;

public class PlayerStatsCalculatorFactory : IPlayerStatsCalculatorFactory
{
    private readonly IEnumerable<IPlayerStatsCalculator> _calculators;

    public PlayerStatsCalculatorFactory(IEnumerable<IPlayerStatsCalculator> calculators)
    {
        _calculators = calculators;
    }

    public IPlayerStatsCalculator GetCalculator(string sportName)
    {
        var calculator = _calculators.FirstOrDefault(c => c.SportName == sportName);

        if(calculator is null)
        {
            throw new NotSupportedException($"Nema kalkulatora statistike za sport: {sportName}");
        }

        return calculator;
    }
}
