import { Injectable, signal } from '@angular/core';

export interface FeedbackMessage {
  type: 'success' | 'error';
  text: string;
}

@Injectable({ providedIn: 'root' })
export class AdminFeedbackService {
  private messageSignal = signal<FeedbackMessage | null>(null);
  message = this.messageSignal.asReadonly();

  success(text: string) {
    this.messageSignal.set({ type: 'success', text });
  }

  // čita poruku koju vraća naš GlobalExceptionHandler: { "error": "..." }
  error(err: any, fallback: string) {
    this.messageSignal.set({ type: 'error', text: err?.error?.error ?? fallback });
  }

  clear() {
    this.messageSignal.set(null);
  }
}