import { Component, inject, input, output, signal } from '@angular/core';
import { UploadService } from '../../../services/upload';
import { uploadErrorMessage, validateImageFile } from '../../../utils/image-file';
import { MediaUrlPipe } from '../../../pipes/media-url';

@Component({
  selector: 'app-editable-avatar',
  templateUrl: './editable-avatar.html',
  styleUrl: './editable-avatar.scss',
  imports:[MediaUrlPipe]
})
export class EditableAvatar {
  private uploadService = inject(UploadService);

  imageUrl = input<string | null>(null);
  fallbackText = input('');
  shape = input<'circle' | 'square'>('circle');

  imageChanged = output<string>();
  failed = output<string>();

  busy = signal(false);

  onFileSelected(event: Event) {
    const fileInput = event.target as HTMLInputElement;
    const file = fileInput.files?.[0];
    fileInput.value = '';

    if (!file) {
      return;
    }

    const problem = validateImageFile(file);
    if (problem) {
      this.failed.emit(problem);
      return;
    }

    this.busy.set(true);

    this.uploadService.uploadFile(file).subscribe({
      next: result => {
        this.busy.set(false);
        this.imageChanged.emit(result.path);
      },
      error: err => {
        this.busy.set(false);
        this.failed.emit(uploadErrorMessage(err));
      }
    });
  }
}