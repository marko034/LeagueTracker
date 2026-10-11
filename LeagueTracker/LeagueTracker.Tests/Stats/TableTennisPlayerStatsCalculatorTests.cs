
using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Services.Stats;

namespace LeagueTracker.Tests.Stats;

public class TableTennisPlayerStatsCalculatorTests
{
    private readonly TableTennisPlayerStatsCalculator _calculator = new();

    private static Player MakePlayer(int id, string firstName) =>
        new() { Id = id, FirstName = firstName, LastName = "Test"};

    [Fact]
    public void Calculate_ReturnsFourColumns_WinsLossesSetsWonSetsLost()
    {
        var players = new List<Player> { MakePlayer(1, "A") };
        var matches = new List<Match>();

        var result = _calculator.Calculate(players, matches);

        Assert.Equal(4, result.Columns.Count);
        Assert.Contains(result.Columns, c => c.Key == "wins");
        Assert.Contains(result.Columns, c => c.Key == "losses");
        Assert.Contains(result.Columns, c => c.Key == "setsWon");
        Assert.Contains(result.Columns, c => c.Key == "setsLost");
    }

    [Fact]
    public void Calculate_SinglesGameWins_UpdateOnlyWinningPlayer()
    {
        //Arrange
        var players = new List<Player> { MakePlayer(1, "Petar"), MakePlayer(2, "Marko") };

        var game = new Game
        {
            Type = GameType.Single,
            HomePlayer1Id = 1,
            AwayPlayer1Id = 2,
            Sets = new List<GameSet>
            {
                new() { SetNumber = 1, HomeScore = 11, AwayScore =5 },
                new() { SetNumber = 2, HomeScore = 11, AwayScore =8 }
            }
        };

        var match = new Match
        {
            Status = MatchStatus.Verified,
            Games = new List<Game> { game }
        };

        //Act
        var result = _calculator.Calculate(players, new List<Match> { match });


        //Assert
        var petar = result.Rows.Single(r => r.PlayerId == 1);
        var marko = result.Rows.Single(r => r.PlayerId == 2);

        Assert.Equal(1, petar.Values["wins"]);
        Assert.Equal(0, petar.Values["losses"]);
        Assert.Equal(2, petar.Values["setsWon"]);
        Assert.Equal(0, petar.Values["setsLost"]);

        Assert.Equal(0, marko.Values["wins"]);
        Assert.Equal(1, marko.Values["losses"]);
        Assert.Equal(0, marko.Values["setsWon"]);
        Assert.Equal(2, marko.Values["setsLost"]);
    }

    [Fact]
    public void Calculate_DoublesGame_CreditsBothPlayersOnEachSide()
    {
        //Arrange
        var players = new List<Player> { MakePlayer(1, "Petar"), MakePlayer(2, "Marko"), MakePlayer(3, "Janko"), MakePlayer(4, "Gera") };

        var game = new Game
        {
            HomePlayer1Id = 1,
            HomePlayer2Id = 3,
            AwayPlayer1Id = 2,
            AwayPlayer2Id = 4,
            Type = GameType.Double,
            Sets = new List<GameSet>
            {
                new(){ SetNumber = 1, HomeScore = 11, AwayScore = 7},
                new(){ SetNumber = 2, HomeScore = 6, AwayScore = 11},
                new(){ SetNumber = 3, HomeScore =11, AwayScore = 6}
            }
        };

        var match = new Match
        {
            Status = MatchStatus.Verified,
            Games = new List<Game> { game }
        };

        //Act
        var result = _calculator.Calculate(players, new List<Match> { match } );

        Assert.All(result.Rows.Where(r => r.PlayerId is 1 or 3), row =>
        {
            Assert.Equal(1, row.Values["wins"]);
            Assert.Equal(2, row.Values["setsWon"]);
            Assert.Equal(1, row.Values["setsLost"]);
        });

        Assert.All(result.Rows.Where(r => r.PlayerId is 2 or 4), row =>
        {
            Assert.Equal(1, row.Values["losses"]);
            Assert.Equal(1, row.Values["setsWon"]);
            Assert.Equal(2, row.Values["setsLost"]);
        });
    }

    [Fact]
    public void Calculate_WhenMatchNotVerified_GamesAreIgnored()
    {
        //Arrange
        var players = new List<Player> { MakePlayer(1, "Petar"), MakePlayer(2, "Marko"), MakePlayer(3, "Janko"), MakePlayer(4, "Gera") };

        var game = new Game
        {
            HomePlayer1Id = 1,
            HomePlayer2Id = 3,
            AwayPlayer1Id = 2,
            AwayPlayer2Id = 4,
            Type = GameType.Double,
            Sets = new List<GameSet>
            {
                new(){ SetNumber = 1, HomeScore = 11, AwayScore = 7},
                new(){ SetNumber = 2, HomeScore = 6, AwayScore = 11},
                new(){ SetNumber = 3, HomeScore =11, AwayScore = 6}
            }
        };

        var match = new Match
        {
            Status = MatchStatus.AwaitingVerification,
            Games = new List<Game> { game }
        };

        //Act
        var result = _calculator.Calculate(players, new List<Match> { match } );

        Assert.All(result.Rows, row =>
        {
            Assert.Equal(0, row.Values["wins"]);
            Assert.Equal(0, row.Values["setsWon"]);
        });
    }

    [Fact]
    public void Calculate_AccumulatesStatsAcrossMultipleGames()
    {
        //Arrange
        var players = new List<Player> { MakePlayer(1, "Petar"), MakePlayer(2, "Marko"), MakePlayer(3, "Janko") };

        var game = new Game
        {
            Type = GameType.Single,
            HomePlayer1Id = 1,
            AwayPlayer1Id = 2,
            Sets = new List<GameSet>
            {
                new() { SetNumber = 1, HomeScore = 11, AwayScore =5 },
                new() { SetNumber = 2, HomeScore = 11, AwayScore =8 }
            }
        };


        var game2 = new Game
        {
            Type = GameType.Single,
            HomePlayer1Id = 1,
            AwayPlayer1Id = 3,
            Sets = new List<GameSet>
            {
                new() { SetNumber = 1, HomeScore = 5, AwayScore = 11 },
                new() { SetNumber = 2, HomeScore = 8, AwayScore = 11 }
            }
        };

        var match = new Match
        {
            Status = MatchStatus.Verified,
            Games = new List<Game> { game, game2 }
        };

        //Act
        var result = _calculator.Calculate(players, new List<Match> { match });

        //Assert
        var petar = result.Rows.Single(r => r.PlayerId == 1);

        Assert.Equal(1, petar.Values["wins"]);
        Assert.Equal(1, petar.Values["losses"]);
        Assert.Equal(2, petar.Values["setsWon"]);
        Assert.Equal(2, petar.Values["setsLost"]);
    }
}
