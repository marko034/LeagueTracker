import { Component, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { AnnouncementService } from '../../services/announcementservice';

@Component({
  selector: 'app-news',
  imports: [DatePipe],
  templateUrl: './news.html',
  styleUrl: './news.scss'
})
export class News {
  private announcementService = inject(AnnouncementService);
  announcements = this.announcementService.announcements;
  loading = this.announcementService.loading;

  constructor() {
    this.announcementService.loadRecent();
  }
}