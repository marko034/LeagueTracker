import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Player } from '../models/player';

@Injectable({
  providedIn: 'root',
})
export class PlayerService {
  getPlayersRaw(teamId: number, seasonId: number) {
    return this.http.get<Player[]>(`${this.apiUrl}?teamId=${teamId}&seasonId=${seasonId}`);
  }

  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5296/api/players';

  private playersSignal = signal<Player[]>([]);
  players = this.playersSignal.asReadonly();

  loading = signal(false);

  loadByTeam(teamId: number, seasonId: number) {
    this.loading.set(true);
    this.http.get<Player[]>(`${this.apiUrl}?teamId=${teamId}&seasonId=${seasonId}`).subscribe({
      next: players => {
        this.playersSignal.set(players);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  getById(id: number, seasonId?: number | null) {
    const url = seasonId ? `${this.apiUrl}/${id}?seasonId=${seasonId}` : `${this.apiUrl}/${id}`;
    return this.http.get<Player>(url);
  }

  create(dto: { firstName: string; lastName: string; photoUrl: string | null; dateOfBirth: string | null }) {
    return this.http.post<Player>(this.apiUrl, dto);
  }

  update(id: number, dto: { firstName: string; lastName: string; photoUrl: string | null; dateOfBirth: string | null }) {
    return this.http.put<Player>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getAllRaw() {
    return this.http.get<Player[]>(this.apiUrl);
  }

  register(playerId: number, teamId: number, seasonId: number) {
    return this.http.post(`${this.apiUrl}/${playerId}/register`, { teamId, seasonId });
  }

  unregister(playerId: number, seasonId: number) {
    return this.http.delete(`${this.apiUrl}/${playerId}/register?seasonId=${seasonId}`);
  }

  getBySeasonRaw(seasonId: number) {
    return this.http.get<Player[]>(`${this.apiUrl}?seasonId=${seasonId}`);
  }
}
