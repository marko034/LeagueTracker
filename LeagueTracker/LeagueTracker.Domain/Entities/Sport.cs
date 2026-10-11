using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueTracker.Domain.Entities;

public class Sport
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<League> Leagues { get; set; } = new List<League>();
}

