import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Sport } from '../models/sport';

@Injectable({
  providedIn: 'root',
})
export class SportService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5296/api/sports';

  private sportsSignal = signal<Sport[]>([]);
  sports = this.sportsSignal.asReadonly();

  loadSports() {
    this.http.get<Sport[]>(this.apiUrl).subscribe({
      next: sports => this.sportsSignal.set(sports),
      error: err => console.error('Greska pri ucitavanju sportova:', err)
    });
  }
}
