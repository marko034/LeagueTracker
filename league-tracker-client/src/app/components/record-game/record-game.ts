import { Component, effect, inject, input, signal } from '@angular/core';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatchService } from '../../services/match-service';
import { PlayerService } from '../../services/player';
import { GameService } from '../../services/game-service';
import { Router } from '@angular/router';
import { GameType } from '../../models/create-game';
import { Player } from '../../models/player';
import { AuthService } from '../../services/auth-service';

type SetFormGroup = FormGroup<{
  setNumber: FormControl<number>;
  homeScore: FormControl<number>;
  awayScore: FormControl<number>;
}>;

@Component({
  selector: 'app-record-game',
  imports: [ReactiveFormsModule],
  templateUrl: './record-game.html',
  styleUrl: './record-game.scss',
})
export class RecordGame {
  private matchService = inject(MatchService);
  private gameService = inject(GameService);
  private router = inject(Router);
  private playerService = inject(PlayerService);
  authService = inject(AuthService);
  matchId = input.required<number>();

  homeTeamId = signal<number | null>(null);
  awayTeamId = signal<number | null>(null);

  homePlayers = signal<Player[]>([]);
  awayPlayers = signal<Player[]>([]);

  submitting = signal(false);
  submitError = signal<string | null>(null);

  form = new FormGroup({
    type: new FormControl<GameType>('Single', { nonNullable: true }),
    homePlayer1Id: new FormControl<number | null>(null, Validators.required),
    homePlayer2Id: new FormControl<number | null>(null),
    awayPlayer1Id: new FormControl<number | null>(null, Validators.required),
    awayPlayer2Id: new FormControl<number | null>(null),
    sets: new FormArray<SetFormGroup>([])
  });

  constructor() {
    effect(() => {
      const id = this.matchId();
      this.matchService.getById(id).subscribe(match => {
        this.homeTeamId.set(match.homeTeamId);
        this.awayTeamId.set(match.awayTeamId);
        this.playerService.getPlayersRaw(match.homeTeamId, match.seasonId).subscribe(players => this.homePlayers.set(players));
        this.playerService.getPlayersRaw(match.awayTeamId, match.seasonId).subscribe(players => this.awayPlayers.set(players));
      });
    });
    this.addSet();
  }

  get sets(): FormArray<SetFormGroup> {
    return this.form.controls.sets;
  }

  get isDoubles(): boolean {
    return this.form.controls.type.value === 'Double';
  }

  addSet() {
    const setGroup: SetFormGroup = new FormGroup({
      setNumber: new FormControl(this.sets.length + 1, { nonNullable: true }),
      homeScore: new FormControl(0, { nonNullable: true, validators: [Validators.required, Validators.min(0)] }),
      awayScore: new FormControl(0, { nonNullable: true, validators: [Validators.required, Validators.min(0)] })
    });
    this.sets.push(setGroup);
  }

  removeSet(index: number) {
    this.sets.removeAt(index);
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.submitError.set(null);

    const value = this.form.getRawValue();

    this.gameService.recordGame({
      matchId: this.matchId(),
      type: value.type,
      homePlayer1Id: Number(value.homePlayer1Id),
      homePlayer2Id: value.homePlayer2Id !== null ? Number(value.homePlayer2Id) : null,
      awayPlayer1Id: Number(value.awayPlayer1Id),
      awayPlayer2Id: value.awayPlayer2Id !== null ? Number(value.awayPlayer2Id) : null,
      sets: value.sets.map(s => ({
        setNumber: Number(s.setNumber),
        homeScore: Number(s.homeScore),
        awayScore: Number(s.awayScore)
      }))
    }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigate(['/matches', this.matchId(), 'record-game']);
      },
      error: (err) => {
        this.submitting.set(false);
        if (err.status === 403) {
          this.submitError.set(err.error?.error ?? 'Nemate pravo da unesete rezultat ove partije.');
        } else if (err.status === 401) {
          this.submitError.set('Morate biti prijavljeni.');
        } else {
          this.submitError.set('Greška pri čuvanju partije.');
        }
      }
    });


  }

  onFinalize() {
    this.gameService.finalizeMatch(this.matchId()).subscribe({
      next: () => alert('Meč finalizovan, čeka verifikaciju.'),
      error: (err) => {
        this.submitting.set(false);
        if (err.status === 403) {
          this.submitError.set(err.error?.error ?? 'Nemate pravo da finalizujete ovaj meč.');
        } else if (err.status === 401) {
          this.submitError.set('Morate biti prijavljeni.');
        } else {
          this.submitError.set('Greška pri finalizaciji meča.');
        }
      }
    });
  }

  onVerify() {
    this.gameService.verifyMatch(this.matchId()).subscribe(() => {
      alert('Meč verifikovan.');
    });
  }
}
