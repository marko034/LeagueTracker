import { Component, computed, effect, inject, OnInit } from '@angular/core';
import { PlayerStatsService } from '../../services/player-stats-service';
import { MatchHub } from '../../services/match-hub';
import { SeasonService } from '../../services/season';

@Component({
  selector: 'app-player-stats',
  imports: [],
  templateUrl: './player-stats.html',
  styleUrl: './player-stats.scss',
})
export class PlayerStats implements OnInit {
  private statsService = inject(PlayerStatsService);
  private matchHub = inject(MatchHub);
  private seasonService = inject(SeasonService);

  table = this.statsService.table;
  loading = this.statsService.loading;

  ngOnInit() {
    this.statsService.loadBySeason(1);
  }

  constructor() {
    effect(() => {
      const seasonId = this.seasonService.selectedId();
      if (seasonId !== null) {
        this.statsService.loadBySeason(seasonId);
      }
    });
    effect(() => {
      const event = this.matchHub.matchUpdated();
      if (event) {
        this.statsService.loadBySeason(event.seasonId);
      }
    });
  }

  topPlayerId = computed(() => {
    const rows = this.table().rows;
    if (rows.length === 0) return null;
    return rows.reduce((best, r) => (r.values['wins'] ?? 0) > (best.values['wins'] ?? 0) ? r : best).playerId;
  });
}
