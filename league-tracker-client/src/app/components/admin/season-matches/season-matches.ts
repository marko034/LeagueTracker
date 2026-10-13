import { DatePipe } from '@angular/common';
import { Component, effect, inject, input, numberAttribute, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Match } from '../../../models/match';
import { Team } from '../../../models/team';
import { AdminFeedbackService } from '../../../services/admin-feedback';
import { MatchService } from '../../../services/match-service';
import { TeamService } from '../../../services/team';
import { DateTimePicker } from '../../shared/date-time-picker/date-time-picker';

@Component({
  selector: 'app-season-matches',
  imports: [ReactiveFormsModule, RouterLink, DatePipe, DateTimePicker],
  templateUrl: './season-matches.html',
  styleUrl: './season-matches.scss'
})
export class SeasonMatches {
  private matchService = inject(MatchService);
  private teamService = inject(TeamService);
  private feedback = inject(AdminFeedbackService);

  matches = signal<Match[]>([]);
  seasonTeams = signal<Team[]>([]);

  seasonId = input.required({ transform: numberAttribute });

  statusLabels: Record<string, string> = {
    Scheduled: 'Zakazan',
    InProgress: 'U toku',
    AwaitingVerification: 'Čeka verifikaciju',
    Verified: 'Verifikovan',
    Disputed: 'Osporen'
  };

  form = new FormGroup({
    homeTeamId: new FormControl<number | null>(null, Validators.required),
    awayTeamId: new FormControl<number | null>(null, Validators.required),
    scheduledAt: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  });

  constructor() {
    effect(() => {
      const seasonId = this.seasonId();
      if (!seasonId) return;

      this.teamService.getRaw(seasonId).subscribe(t => this.seasonTeams.set(t));
      this.loadMatches(seasonId);
    });
  }

  private loadMatches(seasonId: number) {
    this.matchService.getBySeasonRaw(seasonId).subscribe(m => this.matches.set(m));
  }

  create() {
    const seasonId = this.seasonId();
    if (!seasonId || this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.matchService.create({
      seasonId,
      homeTeamId: Number(value.homeTeamId),
      awayTeamId: Number(value.awayTeamId),
      scheduledAt: value.scheduledAt
    }).subscribe({
      next: () => {
        this.feedback.success('Meč je kreiran.');
        this.form.reset();
        this.loadMatches(seasonId);
      },
      error: err => this.feedback.error(err, 'Kreiranje meča nije uspelo.')
    });
  }

  reset(match: Match) {
    const seasonId = this.seasonId();
    if (!seasonId) return;

    const warning = match.status === 'Verified'
      ? 'Meč je verifikovan i ulazi u tabelu i statistiku, pa će se one promeniti. '
      : '';
    const message = `${warning}Poništiti rezultat meča ${match.homeTeamName} – ${match.awayTeamName}? ` +
                    `Sve unete partije biće obrisane, a meč se vraća u status „Zakazan“.`;
    if (!confirm(message)) return;

    this.matchService.reset(match.id).subscribe({
      next: () => {
        this.feedback.success('Rezultat je poništen.');
        this.loadMatches(seasonId);
      },
      error: err => this.feedback.error(err, 'Poništavanje nije uspelo.')
    });
  }

  remove(match: Match) {
    const seasonId = this.seasonId();
    if (!seasonId) return;

    const warning = match.status === 'Verified'
      ? 'Meč je verifikovan i ulazi u tabelu i statistiku, pa će se one promeniti. '
      : '';
    const message = `${warning}Obrisati meč ${match.homeTeamName} – ${match.awayTeamName}? ` +
                    `Biće obrisane i sve njegove partije. Ovo je nepovratno.`;
    if (!confirm(message)) return;

    this.matchService.delete(match.id).subscribe({
      next: () => {
        this.feedback.success('Meč je obrisan.');
        this.loadMatches(seasonId);
      },
      error: err => this.feedback.error(err, 'Brisanje meča nije uspelo.')
    });
  }
}