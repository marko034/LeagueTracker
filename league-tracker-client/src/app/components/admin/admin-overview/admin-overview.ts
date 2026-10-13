import { Component, inject, signal } from '@angular/core';
import { MatchService } from '../../../services/match-service';
import { AdminFeedbackService } from '../../../services/admin-feedback';
import { Match } from '../../../models/match';
import { GameService } from '../../../services/game-service';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-overview',
  imports: [DatePipe, RouterLink, ReactiveFormsModule],
  templateUrl: './admin-overview.html',
  styleUrl: './admin-overview.scss',
})
export class AdminOverview {
  private matchService = inject(MatchService);
  private feedback = inject(AdminFeedbackService);
  private gameService = inject(GameService);
  
  pendingMatches = signal<Match[]>([]);
  loading = signal(false);

  constructor(){
    this.loadPending();
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
    this.gameService.verifyMatch(matchId).subscribe({
      next: () => this.loadPending(),
      error: (err) => this.feedback.error(err, 'Verifikacija nije uspela.')
    });
  }
}
