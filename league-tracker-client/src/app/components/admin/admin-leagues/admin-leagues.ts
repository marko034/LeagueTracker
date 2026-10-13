import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SeasonService } from '../../../services/season';
import { LeagueService } from '../../../services/league-service';
import { AdminFeedbackService } from '../../../services/admin-feedback';
import { SportService } from '../../../services/sport';
import { DateTimePicker } from '../../shared/date-time-picker/date-time-picker';

@Component({
  selector: 'app-admin-leagues',
  imports: [ReactiveFormsModule, DateTimePicker],
  templateUrl: './admin-leagues.html',
  styleUrl: './admin-leagues.scss',
})
export class AdminLeagues {

  private seasonService = inject(SeasonService);
  private leagueService = inject(LeagueService);
  private feedback = inject(AdminFeedbackService);
  private sportService = inject(SportService);

  leagues = this.leagueService.leagues;
  sports = this.sportService.sports;

  leagueForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    sportId: new FormControl<number | null>(null, Validators.required)
  });

  seasonForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    startDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    leagueId: new FormControl<number | null>(null, Validators.required)
  });

  onCreateLeague() {
    if (this.leagueForm.invalid) {
      this.leagueForm.markAllAsTouched();
      return;
    }
    const value = this.leagueForm.getRawValue();
    this.leagueService.create({ name: value.name, sportId: Number(value.sportId) }).subscribe({
      next: () => {
        this.feedback.success('Liga kreirana.');
        this.leagueForm.reset();
        this.leagueService.loadAll();
      },
      error: (err) => this.feedback.error(err, 'Kreiranje lige nije uspelo.')
    });
  }

  onCreateSeason() {
    if (this.seasonForm.invalid) {
      this.seasonForm.markAllAsTouched();
      return;
    }
    const value = this.seasonForm.getRawValue();
    this.seasonService.create({
      name: value.name,
      startDate: value.startDate,
      endDate: null,
      leagueId: Number(value.leagueId)
    }).subscribe({
      next: () => {
        this.feedback.success('Sezona kreirana.');
        this.seasonForm.reset();
        this.seasonService.loadAll();
      },
      error: (err) => this.feedback.error(err, 'Kreiranje sezone nije uspelo.')
    });
  }
}
