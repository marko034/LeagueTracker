import { DatePipe } from '@angular/common';
import { Component, effect, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatchService } from '../../services/match-service';
import { MatchHub } from '../../services/match-hub';
import { AuthService } from '../../services/auth-service';
import { SeasonService } from '../../services/season';

@Component({
  selector: 'app-match-list',
  imports: [RouterLink, DatePipe],
  templateUrl: './match-list.html',
  styleUrl: './match-list.scss',
})
export class MatchList implements OnInit {
  ngOnInit(): void {
    this.matchService.loadBySeason(1);
  }
  private matchService = inject(MatchService);
  private matchHubService = inject(MatchHub);
  private seasonService = inject(SeasonService);
  authService = inject(AuthService);

  matches = this.matchService.matches;
  loading = this.matchService.loading;

  constructor() {
    effect(() => {
      const seasonId = this.seasonService.selectedId();
      if (seasonId !== null) {
        this.matchService.loadBySeason(seasonId);
      }
    });
    effect(() => {
      const matchUpdatedEvent = this.matchHubService.matchUpdated();
      if (matchUpdatedEvent) {
        this.matchService.loadBySeason(matchUpdatedEvent.seasonId);
      }
    });
  }
}
