import { Component, computed, effect, inject, input, numberAttribute, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Team } from '../../../models/team';
import { AdminFeedbackService } from '../../../services/admin-feedback';
import { TeamService } from '../../../services/team';
import { MediaUrlPipe } from '../../../pipes/media-url';

@Component({
  selector: 'app-season-teams',
  imports: [ReactiveFormsModule, MediaUrlPipe],
  templateUrl: './season-teams.html',
  styleUrl: './season-teams.scss'
})
export class SeasonTeams {
  private teamService = inject(TeamService);
  private feedback = inject(AdminFeedbackService);

  seasonTeams = signal<Team[]>([]);
  allTeams = signal<Team[]>([]);

  seasonId = input.required({ transform: numberAttribute });

  availableTeams = computed(() => {
    const inSeason = new Set(this.seasonTeams().map(t => t.id));
    return this.allTeams().filter(t => !inSeason.has(t.id));
  });

  form = new FormGroup({
    teamId: new FormControl<number | null>(null, Validators.required)
  });

  constructor() {
    this.teamService.getRaw().subscribe(t => this.allTeams.set(t));

    effect(() => {
      const seasonId = this.seasonId();
      if (seasonId) {
        this.loadSeasonTeams(seasonId);
      }
    });
  }

  private loadSeasonTeams(seasonId: number) {
    this.teamService.getRaw(seasonId).subscribe(t => this.seasonTeams.set(t));
  }

  register() {
    const seasonId = this.seasonId();
    if (!seasonId || this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.teamService.registerToSeason(Number(this.form.getRawValue().teamId), seasonId).subscribe({
      next: () => {
        this.feedback.success('Tim je prijavljen na sezonu.');
        this.form.reset();
        this.loadSeasonTeams(seasonId);
      },
      error: err => this.feedback.error(err, 'Prijava tima nije uspela.')
    });
  }

  unregister(team: Team) {
    const seasonId = this.seasonId();
    if (!seasonId) return;

    const message = `Odjaviti tim „${team.name}“ iz sezone? Njegovi igrači biće odjavljeni iz sastava za ovu sezonu ` +
                    `(sami igrači i njihova istorija ostaju sačuvani).`;
    if (!confirm(message)) return;

    this.teamService.unregisterFromSeason(team.id, seasonId, true).subscribe({
      next: () => {
        this.feedback.success('Tim je odjavljen iz sezone.');
        this.loadSeasonTeams(seasonId);
      },
      error: err => this.feedback.error(err, 'Odjava nije uspela.')
    });
  }
}