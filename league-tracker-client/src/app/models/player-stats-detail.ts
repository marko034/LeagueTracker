export interface RecentGame {
  date: string;
  opponentTeamName: string;
  won: boolean;
  setsWon: number;
  setsLost: number;
}

export interface PlayerStatsDetail {
  wins: number;
  losses: number;
  setsWon: number;
  setsLost: number;
  recentForm: RecentGame[];
  seasonBreakdown: SeasonStats[];
}

export interface SeasonStats {
  seasonId: number;
  seasonName: string;
  leagueName: string;
  wins: number;
  losses: number;
  setsWon: number;
  setsLost: number;
}
