import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Team, TeamHistoryEntry } from '../models/team';

@Injectable({
  providedIn: 'root',
})
export class TeamService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5296/api/teams';

  private teamsSignal = signal<Team[]>([]);
  teams = this.teamsSignal.asReadonly();

  loading = signal(false);

  loadBySeason(seasonId: number) {
    this.loading.set(true);
    this.http.get<Team[]>(`${this.apiUrl}?seasonId=${seasonId}`).subscribe({
      next: teams => {
        this.teamsSignal.set(teams);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  getById(id: number) {
    return this.http.get<Team>(`${this.apiUrl}/${id}`);
  }

  create(dto: { name: string; logoUrl: string | null }) {
    return this.http.post<Team>(this.apiUrl, dto);
  }

  update(id: number, dto: { name: string; logoUrl: string | null }) {
    return this.http.put<Team>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getRaw(seasonId?: number) {
    const url = seasonId ? `${this.apiUrl}?seasonId=${seasonId}` : this.apiUrl;
    return this.http.get<Team[]>(url);
  }

  registerToSeason(teamId: number, seasonId: number) {
    return this.http.post(`${this.apiUrl}/${teamId}/seasons/${seasonId}`, {});
  }

  unregisterFromSeason(teamId: number, seasonId: number) {
    return this.http.delete(`${this.apiUrl}/${teamId}/seasons/${seasonId}`);
  }

  getHistory(teamId: number) {
    return this.http.get<TeamHistoryEntry[]>(`${this.apiUrl}/${teamId}/history`);
  }
}
