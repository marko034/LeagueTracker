import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { PlayerStatsTable } from '../models/player-stats-table';
import { PlayerStatsDetail } from '../models/player-stats-detail';

@Injectable({
  providedIn: 'root',
})
export class PlayerStatsService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5296/api/playerstats';

  private tableSignal = signal<PlayerStatsTable>({ columns: [], rows: [] });
  table = this.tableSignal.asReadonly();

  loading = signal(false);

  loadBySeason(seasonId: number) {
    this.loading.set(true);
    this.http.get<PlayerStatsTable>(`${this.apiUrl}?seasonId=${seasonId}`).subscribe({
      next: (table) => {
        this.tableSignal.set(table);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  getPlayerDetail(playerId: number, seasonId: number | null) {
    const query = seasonId ? `?seasonId=${seasonId}` : '';
    return this.http.get<PlayerStatsDetail>(`${this.apiUrl}/player/${playerId}${query}`);
  }
}