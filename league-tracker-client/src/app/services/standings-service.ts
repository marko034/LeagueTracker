import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { StandingsRow } from '../models/standings-row';

@Injectable({
  providedIn: 'root',
})
export class StandingsService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5296/api/standings';

  private rowsSignal = signal<StandingsRow[]>([]);
  rows = this.rowsSignal.asReadonly();

  loadingSignal = signal(false);

  loadBySeason(seasonId: number) {
    this.loadingSignal.set(true);
    this.http.get<StandingsRow[]>(`${this.apiUrl}?seasonId=${seasonId}`).subscribe({
      next: (rows) => {
        this.rowsSignal.set(rows);
        this.loadingSignal.set(false);
      },
      error: () => this.loadingSignal.set(false)
    });
  }
}
