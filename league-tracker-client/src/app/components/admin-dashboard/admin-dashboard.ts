import { Component, effect, inject, signal } from '@angular/core';
import { GameService } from '../../services/game-service';
import { MatchService } from '../../services/match-service';
import { Match } from '../../models/match';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TeamService } from '../../services/team';
import { PlayerService } from '../../services/player';
import { Upload } from '../../services/upload';
import { SeasonService } from '../../services/season';
import { AnnouncementService } from '../../services/announcementservice';
import { Team } from '../../models/team';
import { Player } from '../../models/player';
import { LeagueService } from '../../services/league-service';
import { SportService } from '../../services/sport';

type AdminTab = 'pending' | 'new-team' | 'register-team' | 'new-player' | 'register-player'
  | 'new-match' | 'announcements' | 'new-league' | 'new-season';

@Component({
  selector: 'app-admin-dashboard',
  imports: [RouterLink, DatePipe, ReactiveFormsModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss',
})
export class AdminDashboard {
  private matchService = inject(MatchService);
  private gameService = inject(GameService);
  private teamService = inject(TeamService);
  private playerService = inject(PlayerService);
  private uploadService = inject(Upload);
  private seasonService = inject(SeasonService);
  private announcementService = inject(AnnouncementService);
  private leagueService = inject(LeagueService);
  private sportService = inject(SportService);

  announcements = this.announcementService.announcements;
  pendingMatches = signal<Match[]>([]);
  loading = signal(false);
  teamLogoUploading = signal(false);
  playerPhotoUploading = signal(false);
  actionError = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  editingTeamId = signal<number | null>(null);
  editingPlayerId = signal<number | null>(null);
  selectedTeamIdForPlayers = signal<number | null>(null);
  photoUploading = signal(false);
  allTeams = signal<Team[]>([]);
  allPlayers = signal<Player[]>([]);
  seasonTeams = signal<Team[]>([]); // timovi sezone izabrane u formi za prijavu igrača

  activeTab = signal<AdminTab>('pending');

  teams = this.teamService.teams;
  players = this.playerService.players;
  leagues = this.leagueService.leagues;
  sports = this.sportService.sports;
  seasons = this.seasonService.seasons;

  readonly timeOptions = Array.from({ length: 41 }, (_, index) => {
    const totalMinutes = 12 * 60 + index * 15;
    const hours = String(Math.floor(totalMinutes / 60)).padStart(2, '0');
    const minutes = String(totalMinutes % 60).padStart(2, '0');

    return `${hours}:${minutes}`;
  });

  announcementForm = new FormGroup({
    title: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    content: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  });

  teamForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    logoUrl: new FormControl('', { nonNullable: true }),
  });

  playerForm = new FormGroup({
    firstName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    lastName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    photoUrl: new FormControl<string | null>(null)
  });

  matchForm = new FormGroup({
    homeTeamId: new FormControl<number | null>(null, Validators.required),
    awayTeamId: new FormControl<number | null>(null, Validators.required),
    scheduledDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    scheduledTime: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  leagueForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    sportId: new FormControl<number | null>(null, Validators.required)
  });

  seasonForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    startDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    leagueId: new FormControl<number | null>(null, Validators.required)
  });

  teamRegistrationForm = new FormGroup({
    teamId: new FormControl<number | null>(null, Validators.required),
    seasonId: new FormControl<number | null>(null, Validators.required)
  });

  playerRegistrationForm = new FormGroup({
    playerId: new FormControl<number | null>(null, Validators.required),
    seasonId: new FormControl<number | null>(null, Validators.required),
    teamId: new FormControl<number | null>(null, Validators.required)
  });

  constructor() {
    this.loadPending();
    this.loadAllTeams();
    this.loadAllPlayers();
    effect(() => {
      const seasonId = this.seasonService.selectedId();
      if (seasonId) {
        this.teamService.loadBySeason(seasonId);
      }
    });
    this.announcementService.loadRecent();
    this.leagueService.loadAll();
    this.sportService.loadSports();
    this.playerRegistrationForm.controls.seasonId.valueChanges.subscribe(seasonId => {
      this.playerRegistrationForm.controls.teamId.reset(null);
      if (seasonId) {
        this.teamService.getRaw(Number(seasonId)).subscribe(t => this.seasonTeams.set(t));
      } else {
        this.seasonTeams.set([]);
      }
    });
  }

  loadAllTeams() {
    this.teamService.getRaw().subscribe(t => this.allTeams.set(t));
  }

  loadAllPlayers() {
    this.playerService.getAllRaw().subscribe(p => this.allPlayers.set(p));
  }

  onTeamFilterChange(teamId: number) {
    this.selectedTeamIdForPlayers.set(teamId);
    const seasonId = this.seasonService.selectedId();
    if (teamId && seasonId)
      this.playerService.loadByTeam(teamId, seasonId);
  }

  onPlayerTeamFilterChange(event: Event) {
    const teamId = Number((event.target as HTMLSelectElement).value);
    this.selectedTeamIdForPlayers.set(teamId || null);

    const seasonId = this.seasonService.selectedId();
    if (teamId && seasonId) {
      this.playerService.loadByTeam(teamId, seasonId);
    }
  }

  startEditTeam(team: Team) {
    this.editingTeamId.set(team.id);
    this.teamForm.patchValue({ name: team.name, logoUrl: team.logoUrl ?? '' });
    this.activeTab.set('new-team');
  }

  startEditPlayer(player: Player) {
    this.editingPlayerId.set(player.id);
    this.playerForm.patchValue({
      firstName: player.firstName,
      lastName: player.lastName,
      photoUrl: player.photoUrl
    });
    this.activeTab.set('new-player');
  }

  cancelEditTeam() {
    this.editingTeamId.set(null);
    this.teamForm.reset();
  }

  cancelEditPlayer() {
    this.editingPlayerId.set(null);
    this.playerForm.reset();
  }

  setTab(tab: AdminTab) {
    this.activeTab.set(tab);
    this.actionError.set(null);
    this.successMessage.set(null);
  }

  private loadPending() {
    this.loading.set(true);
    this.matchService.getAwaitingVerification().subscribe({
      next: matches => {
        this.pendingMatches.set(matches);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onVerify(matchId: number) {
    this.actionError.set(null);
    this.gameService.verifyMatch(matchId).subscribe({
      next: () => this.loadPending(),
      error: () => this.actionError.set('Verifikacija nije uspela.')
    });
  }

  onCreateMatch() {
    if (this.matchForm.invalid) {
      this.matchForm.markAllAsTouched();
      return;
    }

    const seasonId = this.seasonService.selectedId();
    if (seasonId === null) {
      this.actionError.set('Nije izabrana sezona.');
      return;
    }

    const value = this.matchForm.getRawValue();

    this.matchService.create({
      homeTeamId: Number(value.homeTeamId),
      awayTeamId: Number(value.awayTeamId),
      scheduledAt: `${value.scheduledDate}T${value.scheduledTime}:00`,
      seasonId
    }).subscribe({
      next: () => {
        this.successMessage.set('Utakmica je kreirana.');
        this.matchForm.reset();
      },
      error: () => this.actionError.set('Kreiranje utakmice nije uspelo.')
    });
  }

  onTeamLogoSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.teamLogoUploading.set(true);
    this.uploadService.uploadFile(file).subscribe({
      next: result => {
        this.teamForm.patchValue({ logoUrl: result.url });
        this.teamLogoUploading.set(false);
      },
      error: () => {
        this.actionError.set('Upload slike nije uspeo.');
        this.teamLogoUploading.set(false);
      }
    });
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
      error: () => {
        this.actionError.set('Upload slike nije uspeo.');
        this.playerPhotoUploading.set(false);
      }
    });
  }

  onCreateAnnouncement() {
    if (this.announcementForm.invalid) {
      this.announcementForm.markAllAsTouched();
      return;
    }

    const value = this.announcementForm.getRawValue();
    this.announcementService.create({ title: value.title, content: value.content, leagueId: null }).subscribe({
      next: () => {
        this.successMessage.set('Najava objavljena.');
        this.announcementForm.reset();
        this.announcementService.loadRecent();
      },
      error: () => this.actionError.set('Objavljivanje nije uspelo.')
    });
  }

  onDeleteAnnouncement(id: number) {
    this.announcementService.delete(id).subscribe({
      next: () => this.announcementService.loadRecent(),
      error: () => this.actionError.set('Brisanje nije uspelo.')
    });
  }

  onSubmitTeam() {
    if (this.teamForm.invalid) {
      this.teamForm.markAllAsTouched();
      return;
    }

    const value = this.teamForm.getRawValue();
    const dto = { name: value.name, logoUrl: value.logoUrl || null };

    const editingId = this.editingTeamId();
    const request = editingId
      ? this.teamService.update(editingId, dto)
      : this.teamService.create(dto);

    request.subscribe({
      next: () => {
        this.successMessage.set(editingId ? 'Tim izmenjen.' : 'Tim kreiran.');
        this.teamForm.reset();
        this.editingTeamId.set(null);
        this.loadAllTeams();
        this.reloadSeasonTeams();
      },
      error: err => this.showError(err, 'Čuvanje tima nije uspelo.')
    });
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
        this.successMessage.set(editingId ? 'Igrač izmenjen.' : 'Igrač kreiran.');
        this.playerForm.reset();
        this.editingPlayerId.set(null);
        this.loadAllPlayers();
      },
      error: err => this.showError(err, 'Čuvanje igrača nije uspelo.')
    });
  }

  deleteTeam(id: number) {
    if (!confirm('Obrisati tim? Ovo je nepovratno.')) {
      return;
    }

    this.teamService.delete(id).subscribe({
      next: () => { this.loadAllTeams(); this.reloadSeasonTeams(); },
      error: err => this.showError(err, 'Brisanje tima nije uspelo.')
    });
  }

  deleteMatch(id: number) {
    if (!confirm('Obrisati meč? Ovo će obrisati i sve partije unutar njega.')) {
      return;
    }

    this.matchService.delete(id).subscribe({
      next: () => this.matchService.loadBySeason(this.seasonService.selectedId()!),
      error: () => this.actionError.set('Brisanje meča nije uspelo.')
    });
  }

  deletePlayer(id: number) {
    if (!confirm('Obrisati igrača?')) return;

    this.playerService.delete(id).subscribe({
      next: () => this.loadAllPlayers(),
      error: err => this.showError(err, 'Brisanje igrača nije uspelo.')
    });
  }

  onCreateLeague() {
    if (this.leagueForm.invalid) {
      this.leagueForm.markAllAsTouched();
      return;
    }
    const value = this.leagueForm.getRawValue();
    this.leagueService.create({ name: value.name, sportId: Number(value.sportId) }).subscribe({
      next: () => {
        this.successMessage.set('Liga kreirana.');
        this.leagueForm.reset();
        this.leagueService.loadAll();
      },
      error: () => this.actionError.set('Kreiranje lige nije uspelo.')
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
        this.successMessage.set('Sezona kreirana.');
        this.seasonForm.reset();
        this.seasonService.loadAll();
      },
      error: () => this.actionError.set('Kreiranje sezone nije uspelo.')
    });
  }

  private reloadSeasonTeams() {
    const seasonId = this.seasonService.selectedId();
    if (seasonId) this.teamService.loadBySeason(seasonId);
  }

  private showError(err: any, fallback: string) {
    this.actionError.set(err?.error?.error ?? fallback);
  }

  onRegisterPlayer() {
    if (this.playerRegistrationForm.invalid) { this.playerRegistrationForm.markAllAsTouched(); return; }

    const v = this.playerRegistrationForm.getRawValue();
    this.playerService.register(Number(v.playerId), Number(v.teamId), Number(v.seasonId)).subscribe({
      next: () => {
        this.successMessage.set('Igrač prijavljen u sastav.');
        this.playerRegistrationForm.reset();
      },
      error: err => this.showError(err, 'Prijava igrača nije uspela.')
    });
  }

  unregisterPlayer(playerId: number) {
    const seasonId = this.seasonService.selectedId();
    const teamId = this.selectedTeamIdForPlayers();
    if (!seasonId || !teamId || !confirm('Odjaviti igrača iz sastava u izabranoj sezoni?')) return;

    this.playerService.unregister(playerId, seasonId).subscribe({
      next: () => this.playerService.loadByTeam(teamId, seasonId),
      error: err => this.showError(err, 'Odjava igrača nije uspela.')
    });
  }

  onRegisterTeam() {
    if (this.teamRegistrationForm.invalid) { this.teamRegistrationForm.markAllAsTouched(); return; }

    const v = this.teamRegistrationForm.getRawValue();
    this.teamService.registerToSeason(Number(v.teamId), Number(v.seasonId)).subscribe({
      next: () => {
        this.successMessage.set('Tim prijavljen na sezonu.');
        this.teamRegistrationForm.reset();
        this.reloadSeasonTeams();
      },
      error: err => this.showError(err, 'Prijava tima nije uspela.')
    });
  }

  unregisterTeam(teamId: number) {
    const seasonId = this.seasonService.selectedId();
    if (!seasonId || !confirm('Odjaviti tim iz izabrane sezone?')) return;

    this.teamService.unregisterFromSeason(teamId, seasonId).subscribe({
      next: () => this.reloadSeasonTeams(),
      error: err => this.showError(err, 'Odjava nije uspela.')
    });
  }

}