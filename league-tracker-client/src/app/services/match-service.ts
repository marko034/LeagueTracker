import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Match } from '../models/match';

@Injectable({
  providedIn: 'root',
})
export class MatchService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5296/api/matches';

  private matchesSignal = signal<Match[]>([]);
  matches = this.matchesSignal.asReadonly();
  loading = signal(false);

  loadBySeason(seasonId: number) {
    this.loading.set(true);
    this.http.get<Match[]>(`${this.apiUrl}?seasonId=${seasonId}`).subscribe({
      next: matches => {
        this.matchesSignal.set(matches);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  getById(Id: number) {
    return this.http.get<Match>(`${this.apiUrl}/${Id}`);
  }

  getAwaitingVerification() {
    return this.http.get<Match[]>(`${this.apiUrl}/awaiting-verification`);
  }

  create(dto: { seasonId: number; homeTeamId: number; awayTeamId: number; scheduledAt: string }) {
    return this.http.post<Match>(this.apiUrl, dto);
  }

  delete(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getBySeasonRaw(seasonId: number) {
    return this.http.get<Match[]>(`${this.apiUrl}?seasonId=${seasonId}`);
  }

  reset(id: number) {
    return this.http.post(`${this.apiUrl}/${id}/reset`, {});
  }
}