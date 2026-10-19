import { Component, computed, effect, inject, input, numberAttribute, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Player } from '../../../models/player';
import { Team } from '../../../models/team';
import { AdminFeedbackService } from '../../../services/admin-feedback';
import { PlayerService } from '../../../services/player';
import { TeamService } from '../../../services/team';
import { MediaUrlPipe } from '../../../pipes/media-url';

@Component({
  selector: 'app-season-rosters',
  imports: [ReactiveFormsModule, MediaUrlPipe],
  templateUrl: './season-rosters.html',
  styleUrl: './season-rosters.scss'
})
export class SeasonRosters {
  private teamService = inject(TeamService);
  private playerService = inject(PlayerService);
  private feedback = inject(AdminFeedbackService);

  seasonTeams = signal<Team[]>([]);
  roster = signal<Player[]>([]);
  allPlayers = signal<Player[]>([]);
  registeredInSeason = signal<Player[]>([]);

  seasonId = input.required({ transform: numberAttribute });

  teamControl = new FormControl<number | null>(null);
  teamId = toSignal(this.teamControl.valueChanges, { initialValue: null });

  addForm = new FormGroup({
    playerId: new FormControl<number | null>(null, Validators.required)
  });

  availablePlayers = computed(() => {
    const taken = new Set(this.registeredInSeason().map(p => p.id));
    return this.allPlayers().filter(p => !taken.has(p.id));
  });

  constructor() {
    this.playerService.getAllRaw().subscribe(p => this.allPlayers.set(p));

    // promena sezone: resetuj izbor tima i učitaj sve ispočetka
    effect(() => {
      const seasonId = this.seasonId();
      if (!seasonId) return;

      this.teamControl.reset(null);
      this.roster.set([]);
      this.teamService.getRaw(seasonId).subscribe(t => this.seasonTeams.set(t));
      this.loadRegistered(seasonId);
    });

    // promena tima: učitaj njegov sastav
    effect(() => {
      const seasonId = this.seasonId();
      const teamId = this.teamId();
      if (seasonId && teamId) {
        this.loadRoster(Number(teamId), seasonId);
      }
    });
  }

  private loadRoster(teamId: number, seasonId: number) {
    this.playerService.getPlayersRaw(teamId, seasonId).subscribe(p => this.roster.set(p));
  }

  private loadRegistered(seasonId: number) {
    this.playerService.getBySeasonRaw(seasonId).subscribe(p => this.registeredInSeason.set(p));
  }

  register() {
    const seasonId = this.seasonId();
    const teamId = this.teamId();
    if (!seasonId || !teamId || this.addForm.invalid) {
      this.addForm.markAllAsTouched();
      return;
    }

    this.playerService.register(Number(this.addForm.getRawValue().playerId), Number(teamId), seasonId).subscribe({
      next: () => {
        this.feedback.success('Igrač je prijavljen u sastav.');
        this.addForm.reset();
        this.loadRoster(Number(teamId), seasonId);
        this.loadRegistered(seasonId);
      },
      error: err => this.feedback.error(err, 'Prijava igrača nije uspela.')
    });
  }

  unregister(player: Player) {
    const seasonId = this.seasonId();
    const teamId = this.teamId();
    if (!seasonId || !teamId) return;

    if (!confirm(`Odjaviti igrača ${player.firstName} ${player.lastName} iz sastava u ovoj sezoni?`)) return;

    this.playerService.unregister(player.id, seasonId).subscribe({
      next: () => {
        this.feedback.success('Igrač je odjavljen iz sastava.');
        this.loadRoster(Number(teamId), seasonId);
        this.loadRegistered(seasonId);
      },
      error: err => this.feedback.error(err, 'Odjava igrača nije uspela.')
    });
  }
}