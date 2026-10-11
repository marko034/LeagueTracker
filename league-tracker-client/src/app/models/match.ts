export interface Match {
  id: number;
  scheduledAt: string;
  homeTeamId: number;
  homeTeamName: string;
  homeTeamLogoUrl: string | null;
  awayTeamId: number;
  awayTeamName: string;
  awayTeamLogoUrl: string | null;
  status: string;
  homeScore: number | null;
  awayScore: number | null;
  seasonId: number;
}
