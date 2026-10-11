import { Component, inject, OnInit, effect } from '@angular/core';
import { StandingsService } from '../../services/standings-service';
import { MatchHub } from '../../services/match-hub';
import { SeasonService } from '../../services/season';

@Component({
  selector: 'app-standings',
  imports: [],
  templateUrl: './standings.html',
  styleUrl: './standings.scss',
})
export class Standings implements OnInit {
  private standingsService = inject(StandingsService);
  private matchHubService = inject(MatchHub);
  private seasonService = inject(SeasonService);
  rows = this.standingsService.rows;
  loading = this.standingsService.loadingSignal;

  ngOnInit() {
    this.standingsService.loadBySeason(1);
  }

  constructor() {
    effect(() => {
      const seasonId = this.seasonService.selectedId();
      if (seasonId !== null) {
        this.standingsService.loadBySeason(seasonId);
      }
    });
    effect(() => {
      const event = this.matchHubService.matchUpdated();
      if (event) {
        this.standingsService.loadBySeason(event.seasonId);
      }
    });
  }
}
