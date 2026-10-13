import { Component, inject, signal } from '@angular/core';
import { Upload } from '../../../services/upload';
import { TeamService } from '../../../services/team';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Team } from '../../../models/team';
import { AdminFeedbackService } from '../../../services/admin-feedback';
import { SeasonService } from '../../../services/season';

@Component({
  selector: 'app-admin-teams',
  imports: [ReactiveFormsModule],
  templateUrl: './admin-teams.html',
  styleUrl: './admin-teams.scss',
})
export class AdminTeams {
  private teamService = inject(TeamService);
  private uploadService = inject(Upload);
  private feedback = inject(AdminFeedbackService);
  private seasonService = inject(SeasonService);

  loading = signal(false);
  teamLogoUploading = signal(false);
  editingTeamId = signal<number | null>(null);
  selectedTeamIdForPlayers = signal<number | null>(null);
  allTeams = signal<Team[]>([]);

  teams = this.teamService.teams;

  teamForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    logoUrl: new FormControl('', { nonNullable: true }),
  });

  constructor(){
    this.loadAllTeams();
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
      error: (err) => {
        this.feedback.error(err, 'Upload slike nije uspeo.');
        this.teamLogoUploading.set(false);
      }
    });
  }

  startEditTeam(team: Team) {
    this.editingTeamId.set(team.id);
    this.teamForm.patchValue({ name: team.name, logoUrl: team.logoUrl ?? '' });
    //this.activeTab.set('new-team');
  }

  cancelEditTeam() {
    this.editingTeamId.set(null);
    this.teamForm.reset();
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
        this.feedback.success(editingId ? 'Tim izmenjen.' : 'Tim kreiran.');
        this.teamForm.reset();
        this.editingTeamId.set(null);
        this.loadAllTeams();
        this.reloadSeasonTeams();
      },
      error: err => this.feedback.error(err, 'Čuvanje tima nije uspelo.')
    });
  }

  deleteTeam(id: number) {
    if (!confirm('Obrisati tim? Ovo je nepovratno.')) {
      return;
    }

    this.teamService.delete(id).subscribe({
      next: () => { this.loadAllTeams(); this.reloadSeasonTeams(); },
      error: err => this.feedback.error(err, 'Brisanje tima nije uspelo.')
    });
  }

  loadAllTeams() {
    this.teamService.getRaw().subscribe(t => this.allTeams.set(t));
  }

   private reloadSeasonTeams() {
    const seasonId = this.seasonService.selectedId();
    if (seasonId) this.teamService.loadBySeason(seasonId);
  }
}
