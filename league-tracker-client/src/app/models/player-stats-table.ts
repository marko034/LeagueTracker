import { StatColumn } from './stat-column';
import { PlayerStatsRow } from './player-stats-row';

export interface PlayerStatsTable {
  columns: StatColumn[];
  rows: PlayerStatsRow[];
}