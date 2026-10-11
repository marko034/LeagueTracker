using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Services.Standings;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueTracker.Tests.Standings;

public class TableTennisStandingsCalculatorTests
{
    private readonly TableTennisStandingsCalculator _calculator = new();

    [Fact]
    public void Calculate_WhenNoMatche_ReturnAllTeamsWithZeroStats()
    {
        //Arrange
        var teams = new List<Team>
        {
            new(){ Id=1, Name = "Tim A"},
            new(){ Id=2, Name = "Tim B"}
        };

        var matches = new List<Match>();

        //Act
        var result = _calculator.Calculate(matches, teams);

        //Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, row =>
        {
            Assert.Equal(0, row.Played);
            Assert.Equal(0, row.Points);
        });
    }

    [Fact]
    public void Calculate_WhenHomeTeamWins_AwardThreePointsToHomeTeam()
    {
        //Arange
        var teams = new List<Team>
        {
            new(){Id = 1, Name = "Tim A"},
            new(){Id = 2, Name = "Tim B"}
        };

        var matches = new List<Match>
        {
            new()
            {
                HomeTeamId = 1,
                AwayTeamId = 2,
                Status = MatchStatus.Verified,
                HomeScore = 6,
                AwayScore = 4
            }
        };

        //Act
        var result = _calculator.Calculate(matches, teams);

        //Assert
        var teamA = result.Single(r => r.TeamId == 1);
        var teamB = result.Single(r => r.TeamId == 2);

        Assert.Equal(3, teamA.Points);
        Assert.Equal(1, teamA.Wins);
        Assert.Equal(0, teamB.Points);
        Assert.Equal(1, teamB.Losses);
    }

    [Fact]
    public void Calculate_WhenScoresAreEqual_AwardOnePointToEachTeam()
    {
        //Arrange
        var teams = new List<Team>
        {
            new(){Id = 1, Name = "Tim A"},
            new(){Id = 2, Name = "Tim B"}
        };

        var matches = new List<Match>
        {
            new()
            {
                HomeTeamId = 1,
                AwayTeamId = 2,
                Status = MatchStatus.Verified,
                HomeScore = 5,
                AwayScore = 5
            }
        };

        //Act
        var result = _calculator.Calculate(matches, teams);

        //Assert
        Assert.All(result, row => Assert.Equal(1, row.Points));
        Assert.All(result, row => Assert.Equal(1, row.Draws));
    }

    [Fact]
    public void Calculate_WhenMatchIsNotVerified_IsIgnored()
    {
        //Arrange
        var teams = new List<Team>
        {
            new(){Id = 1, Name = "Tim A"},
            new(){Id = 2, Name = "Tim B"}
        };

        var matches = new List<Match>
        {
            new()
            {
                HomeTeamId = 1,
                AwayTeamId = 2,
                Status = MatchStatus.AwaitingVerification,
                HomeScore = 5,
                AwayScore = 5
            }
        };

        //Act
        var result = _calculator.Calculate(matches, teams);

        //Assert
        Assert.All(result, row => Assert.Equal(0,row.Played));
    }

    [Fact]
    public void Calculate_OrdersTeamsByPointsDescending()
    {
        //Arrange
        var teams = new List<Team>
        {
            new() {Id = 1, Name = "Tim A"},
            new() {Id = 2, Name = "Tim B"},
            new() { Id = 3, Name = "Tim C" }
        };

        var matches = new List<Match>
        {
            new()
            {
                HomeTeamId = 1,
                AwayTeamId = 2,
                Status = MatchStatus.Verified,
                HomeScore = 6,
                AwayScore = 4
            },
            new()
            {
                HomeTeamId = 3,
                AwayTeamId = 2,
                Status = MatchStatus.Verified,
                HomeScore = 5,
                AwayScore = 5
            }
        };

        //Act
        var result = _calculator.Calculate(matches, teams);

        //Assert
        Assert.Equal(1, result[0].TeamId);
        Assert.Equal(3, result[1].TeamId);
        Assert.Equal(2, result[2].TeamId);
    }

}
