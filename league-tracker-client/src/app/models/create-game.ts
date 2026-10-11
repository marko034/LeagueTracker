export type GameType = 'Single' | 'Double';

export interface CreateGame {
  matchId: number;
  type: GameType;
  homePlayer1Id: number;
  homePlayer2Id: number | null;
  awayPlayer1Id: number;
  awayPlayer2Id: number | null;
  sets: { setNumber: number; homeScore: number; awayScore: number }[];
}
