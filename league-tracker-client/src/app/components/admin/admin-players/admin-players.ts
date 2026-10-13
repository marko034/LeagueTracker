import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Player } from '../../../models/player';
import { PlayerService } from '../../../services/player';
import { Upload } from '../../../services/upload';
import { SeasonService } from '../../../services/season';
import { AdminFeedbackService } from '../../../services/admin-feedback';
import { TeamService } from '../../../services/team';
import { Team } from '../../../models/team';

@Component({
  selector: 'app-admin-players',
  imports: [ReactiveFormsModule],
  templateUrl: './admin-players.html',
  styleUrl: './admin-players.scss',
})
export class AdminPlayers {
  private playerService = inject(PlayerService);
  private uploadService = inject(Upload);
  private seasonService = inject(SeasonService);
  private feedback = inject(AdminFeedbackService);
  private teamService = inject(TeamService);

  loading = signal(false);
  playerPhotoUploading = signal(false);
  editingPlayerId = signal<number | null>(null);
  selectedTeamIdForPlayers = signal<number | null>(null);
  photoUploading = signal(false);

  teams = this.teamService.teams;
  search = signal('');

  playerForm = new FormGroup({
    firstName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    lastName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    photoUrl: new FormControl<string | null>(null)
  });

  seasons = this.seasonService.seasons;

  filterTeams = signal<Team[]>([]);
  players = signal<Player[]>([]);
  seasonFilterId = signal<number | null>(null);
  teamFilterId = signal<number | null>(null);

  filteredPlayers = computed(() => {
    const q = this.search().trim().toLowerCase();
    const players = this.players();
    return q ? players.filter(p => `${p.firstName} ${p.lastName}`.toLowerCase().includes(q)) : players;
  });

  constructor() {
    // izabrana sezona: učitaj njene timove za drugi filter
    effect(() => {
      const seasonId = this.seasonFilterId();
      if (seasonId) {
        this.teamService.getRaw(seasonId).subscribe(t => this.filterTeams.set(t));
      } else {
        this.filterTeams.set([]);
      }
    });

    // bilo koji filter se promeni: učitaj listu
    effect(() => this.load(this.seasonFilterId(), this.teamFilterId()));
  }

  onSeasonFilter(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this.seasonFilterId.set(value ? Number(value) : null);
    this.teamFilterId.set(null); // tim iz druge sezone više ne važi
  }

  onTeamFilter(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this.teamFilterId.set(value ? Number(value) : null);
  }

  reload() {
    this.load(this.seasonFilterId(), this.teamFilterId());
  }

  private load(seasonId: number | null, teamId: number | null) {
    const request = seasonId && teamId
      ? this.playerService.getPlayersRaw(teamId, seasonId)
      : seasonId
        ? this.playerService.getBySeasonRaw(seasonId)
        : this.playerService.getAllRaw();

    request.subscribe(p => this.players.set(p));
  }

  onPlayerPhotoSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.playerPhotoUploading.set(true);
    this.uploadService.uploadFile(file).subscribe({
      next: result => {
        this.playerForm.patchValue({ photoUrl: result.url });
        this.playerPhotoUploading.set(false);
      },
      error: (err) => {
        this.feedback.error(err, 'Upload slike nije uspeo.');
        this.playerPhotoUploading.set(false);
      }
    });
  }

  startEditPlayer(player: Player) {
    this.editingPlayerId.set(player.id);
    this.playerForm.patchValue({
      firstName: player.firstName,
      lastName: player.lastName,
      photoUrl: player.photoUrl
    });
    //this.activeTab.set('new-player');
  }

  cancelEditPlayer() {
    this.editingPlayerId.set(null);
    this.playerForm.reset();
  }

  onSubmitPlayer() {
    if (this.playerForm.invalid) {
      this.playerForm.markAllAsTouched();
      return;
    }

    const value = this.playerForm.getRawValue();
    const dto = {
      firstName: value.firstName,
      lastName: value.lastName,
      photoUrl: value.photoUrl,
      dateOfBirth: null
    };

    const editingId = this.editingPlayerId();
    const request = editingId
      ? this.playerService.update(editingId, dto)
      : this.playerService.create(dto);

    request.subscribe({
      next: () => {
        this.feedback.success(editingId ? 'Igrač izmenjen.' : 'Igrač kreiran.');
        this.playerForm.reset();
        this.editingPlayerId.set(null);
        this.reload();
      },
      error: err => this.feedback.error(err, 'Čuvanje igrača nije uspelo.')
    });
  }

  deletePlayer(id: number) {
    if (!confirm('Obrisati igrača?')) return;

    this.playerService.delete(id).subscribe({
      next: () => this.reload(),
      error: err => this.feedback.error(err, 'Brisanje igrača nije uspelo.')
    });
  }

  onPlayerTeamFilterChange(event: Event) {
    const teamId = Number((event.target as HTMLSelectElement).value);
    this.selectedTeamIdForPlayers.set(teamId || null);

    const seasonId = this.seasonService.selectedId();
    if (teamId && seasonId) {
      this.playerService.loadByTeam(teamId, seasonId);
    }
  }

  unregisterPlayer(playerId: number) {
    const seasonId = this.seasonService.selectedId();
    const teamId = this.selectedTeamIdForPlayers();
    if (!seasonId || !teamId || !confirm('Odjaviti igrača iz sastava u izabranoj sezoni?')) return;

    this.playerService.unregister(playerId, seasonId).subscribe({
      next: () => this.playerService.loadByTeam(teamId, seasonId),
      error: err => this.feedback.error(err, 'Odjava igrača nije uspela.')
    });
  }

}
