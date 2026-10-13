import { Component, inject } from '@angular/core';
import { AnnouncementService } from '../../../services/announcementservice';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminFeedbackService } from '../../../services/admin-feedback';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-admin-announcements',
  imports: [DatePipe, ReactiveFormsModule],
  templateUrl: './admin-announcements.html',
  styleUrl: './admin-announcements.scss',
})
export class AdminAnnouncements {
  private announcementService = inject(AnnouncementService);
  private feedback = inject(AdminFeedbackService);

  announcements = this.announcementService.announcements;


  announcementForm = new FormGroup({
    title: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    content: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  });

  onCreateAnnouncement() {
    if (this.announcementForm.invalid) {
      this.announcementForm.markAllAsTouched();
      return;
    }

    const value = this.announcementForm.getRawValue();
    this.announcementService.create({ title: value.title, content: value.content, leagueId: null }).subscribe({
      next: () => {
        this.feedback.success('Najava objavljena.');
        this.announcementForm.reset();
        this.announcementService.loadRecent();
      },
      error: (err) => this.feedback.error(err, 'Objavljivanje nije uspelo.')
    });
  }

  onDeleteAnnouncement(id: number) {
    this.announcementService.delete(id).subscribe({
      next: () => this.announcementService.loadRecent(),
      error: (err) => this.feedback.error(err, 'Brisanje nije uspelo.')
    });
  }
}
