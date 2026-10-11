// services/announcement.ts
import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Announcement } from '../models/announcement';

@Injectable({
  providedIn: 'root'
})
export class AnnouncementService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5296/api/Announcements';

  private announcementsSignal = signal<Announcement[]>([]);
  announcements = this.announcementsSignal.asReadonly();
  loading = signal(false);

  loadRecent() {
    this.loading.set(true);
    this.http.get<Announcement[]>(this.apiUrl).subscribe({
      next: list => {
        this.announcementsSignal.set(list);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  create(dto: { title: string; content: string; leagueId: number | null }) {
    return this.http.post<Announcement>(this.apiUrl, dto);
  }

  delete(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}