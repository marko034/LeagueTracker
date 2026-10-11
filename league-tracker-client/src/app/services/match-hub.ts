import { effect, inject, Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { SeasonService } from './season';

export interface MatchUpdatedEvent {
  matchId: number;
  seasonId: number;
}

@Injectable({
  providedIn: 'root',
})
export class MatchHub {
  private seasonService = inject(SeasonService);
  private connection: signalR.HubConnection;
  private currentGroupSeasonId: number | null = null;

  matchUpdated = signal<MatchUpdatedEvent | null>(null);

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5296/hubs/matches')
      .withAutomaticReconnect()
      .build();

    this.connection.on('MatchUpdated', (event: MatchUpdatedEvent) => {
      this.matchUpdated.set(event);
    });

    this.connection.start()
      .then(() => this.syncGroup(this.seasonService.selectedId()))
      .catch(err => console.error('SignalR konekcija nije uspela:', err));

    effect(() => {
      const seasonId = this.seasonService.selectedId();
      if (this.connection.state === signalR.HubConnectionState.Connected) {
        this.syncGroup(seasonId);
      }
    });

    // Kad se konekcija ponovo uspostavi (automatic reconnect), moramo ponovo da se pridružimo grupi
    this.connection.onreconnected(() => {
      this.currentGroupSeasonId = null; // resetuj, da syncGroup zna da mora ponovo da pozove server
      this.syncGroup(this.seasonService.selectedId());
    });
  }


  syncGroup(newSeasonId: number | null): any {
    if (newSeasonId === this.currentGroupSeasonId) {
      return; // već smo u toj grupi, nema potrebe za pozivom
    }

    const previousSeasonId = this.currentGroupSeasonId;

    if (previousSeasonId !== null) {
      this.connection.invoke('LeaveSeasonGroup', previousSeasonId).catch(err => console.error(err));
    }

    if (newSeasonId !== null) {
      this.connection.invoke('JoinSeasonGroup', newSeasonId).catch(err => console.error(err));
    }

    this.currentGroupSeasonId = newSeasonId;
  }
}
