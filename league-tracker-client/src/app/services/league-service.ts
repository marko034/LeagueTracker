import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { League } from '../models/league';

@Injectable({ providedIn: 'root' })
export class LeagueService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5296/api/Leagues';

  private leaguesSignal = signal<League[]>([]);
  leagues = this.leaguesSignal.asReadonly();

  loadAll() {
    this.http.get<League[]>(this.apiUrl).subscribe(leagues => this.leaguesSignal.set(leagues));
  }

  create(dto: { name: string; sportId: number }) {
    return this.http.post<League>(this.apiUrl, dto);
  }
}