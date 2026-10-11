export interface Season {
  id: number;
  name: string;
  startDate: string;
  endDate: string | null;
  leagueId: number;
  leagueName: string;
}