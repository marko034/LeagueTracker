export interface Team {
  id: number;
  name: string;
  logoUrl: string | null;
}

export interface TeamHistoryEntry {
  seasonId: number;
  seasonName: string;
  leagueName: string;
  position: number | null;
  played: number;
  wins: number;
  draws: number;
  losses: number;
  points: number;
}