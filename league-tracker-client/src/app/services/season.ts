import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Season } from '../models/season';

const SELECTED_SEASON_KEY = 'league_tracker_season_id';

@Injectable({
  providedIn: 'root',
})
export class SeasonService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5296/api/Seasons';

  private seasonsSignal = signal<Season[]>([]);
  seasons = this.seasonsSignal.asReadonly();

  private selectedIdSignal = signal<number | null>(
    Number(localStorage.getItem(SELECTED_SEASON_KEY)) || null
  );
  selectedId = this.selectedIdSignal.asReadonly();

  selectedSeason = computed(() =>
    this.seasonsSignal().find(s => s.id === this.selectedIdSignal()) ?? null
  );

  loadAll() {
    this.http.get<Season[]>(this.apiUrl).subscribe(seasons => {
      this.seasonsSignal.set(seasons);

      // Ako nema izabrane sezone (ili izabrana više ne postoji), izaberi najnoviju
      const current = this.selectedIdSignal();
      if (!current || !seasons.some(s => s.id === current)) {
        if (seasons.length > 0) {
          this.selectSeason(seasons[0].id);
        }
      }
    });
  }

  selectSeason(id: number) {
    this.selectedIdSignal.set(id);
    localStorage.setItem(SELECTED_SEASON_KEY, String(id));
  }

  create(dto: { name: string; startDate: string; endDate: string | null; leagueId: number }) {
    return this.http.post<Season>(this.apiUrl, dto);
  }
}
