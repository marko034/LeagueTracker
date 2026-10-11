import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PlayerService } from '../../services/player';
import { DatePipe } from '@angular/common';
import { Player } from '../../models/player';
import { PlayerStatsService } from '../../services/player-stats-service';
import { SeasonService } from '../../services/season';
import { PlayerStatsDetail } from '../../models/player-stats-detail';
import { StatDonutChart } from '../stat-donut-chart/stat-donut-chart';

@Component({
  selector: 'app-player-detail',
  imports: [RouterLink, DatePipe, StatDonutChart],
  templateUrl: './player-detail.html',
  styleUrl: './player-detail.scss',
})
export class PlayerDetail {
  private playerService = inject(PlayerService);
  private statsService = inject(PlayerStatsService);
  private seasonService = inject(SeasonService);

  id = input.required<number>();
  player = signal<Player | undefined>(undefined);
  stats = signal<PlayerStatsDetail | null>(null);
  scope = signal<'season' | 'career'>('season');

  winRate = computed(() => {
    const s = this.stats();
    if (!s || s.wins + s.losses === 0) return 0;
    return Math.round((s.wins / (s.wins + s.losses)) * 100);
  });

  setRate = computed(() => {
    const s = this.stats();
    if (!s || s.setsWon + s.setsLost === 0) return 0;
    return Math.round((s.setsWon / (s.setsWon + s.setsLost)) * 100);
  });

  constructor() {
    effect(() => {
      this.playerService.getById(this.id(), this.seasonService.selectedId())
        .subscribe(p => this.player.set(p));
    });

    effect(() => {
      const seasonId = this.scope() === 'season' ? this.seasonService.selectedId() : null;
      if (this.scope() === 'season' && !seasonId) return;

      this.statsService.getPlayerDetail(this.id(), seasonId).subscribe(s => this.stats.set(s));
    });
  }
}
