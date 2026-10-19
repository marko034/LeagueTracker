import { Component, effect, inject, input, signal } from '@angular/core';
import { TeamService } from '../../services/team';
import { PlayerService } from '../../services/player';
import { Team, TeamHistoryEntry } from '../../models/team';
import { RouterLink } from '@angular/router';
import { SeasonService } from '../../services/season';
import { MediaUrlPipe } from '../../pipes/media-url';

@Component({
  selector: 'app-team-detail',
  imports: [RouterLink, MediaUrlPipe],
  templateUrl: './team-detail.html',
  styleUrl: './team-detail.scss',
})
export class TeamDetail {
  private teamService = inject(TeamService);
  private playerService = inject(PlayerService);
  private seasonService = inject(SeasonService);

  id = input.required<number>();

  team = signal<Team | undefined>(undefined);
  history = signal<TeamHistoryEntry[]>([]);
  players = this.playerService.players;
  loading = this.teamService.loading;

  constructor() {
    effect(() => {
      const teamId = this.id();
      this.teamService.getById(teamId).subscribe(team => this.team.set(team));
      this.teamService.getHistory(teamId).subscribe(h => this.history.set(h));
    });
    effect(() => {
      const teamId = this.id();
      const seasonId = this.seasonService.selectedId();
      this.teamService.getById(teamId).subscribe(team => this.team.set(team));
      if(teamId && seasonId)
        this.playerService.loadByTeam(teamId, seasonId);
    });
  }
}
