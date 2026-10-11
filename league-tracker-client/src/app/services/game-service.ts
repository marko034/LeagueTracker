import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CreateGame } from '../models/create-game';

@Injectable({
  providedIn: 'root',
})
export class GameService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5296/api/games';

  recordGame(dto: CreateGame){
    return this.http.post(`${this.apiUrl}`, dto);
  }

  finalizeMatch(matchId: number){
    return this.http.post(`${this.apiUrl}/finalize-match/${matchId}`, {});
  }

  verifyMatch(matchId: number){
    return this.http.post(`${this.apiUrl}/verify-match/${matchId}`, {});
  }
}
