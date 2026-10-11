import { Component, effect, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TeamService } from '../../services/team';
import { SeasonService } from '../../services/season';

@Component({
  selector: 'app-team-list',
  imports: [RouterLink],
  templateUrl: './team-list.html',
  styleUrl: './team-list.scss',
})
export class TeamList implements OnInit {
  ngOnInit(): void {
    this.teamService.loadBySeason(1);
  }

  constructor(){
    effect(() => {
      const seasonId = this.seasonService.selectedId();
      if (seasonId !== null) {
        this.teamService.loadBySeason(seasonId);
      }
    });
  }
  private teamService = inject(TeamService);
  private seasonService = inject(SeasonService);
  teams = this.teamService.teams;
  loading = this.teamService.loading;
}
