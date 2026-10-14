import { Component, forwardRef, inject, input, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { UploadService } from '../../../services/upload';
import { uploadErrorMessage, validateImageFile } from '../../../utils/image-file';

@Component({
  selector: 'app-image-upload',
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.scss',
  providers: [
    { provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => ImageUpload), multi: true }
  ]
})
export class ImageUpload implements ControlValueAccessor {
  private uploadService = inject(UploadService);

  shape = input<'circle' | 'square'>('square');
  fallbackText = input('');

  value = signal<string | null>(null);
  uploading = signal(false);
  dragging = signal(false);
  disabled = signal(false);
  error = signal<string | null>(null);

  private onChange: (value: string | null) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: string | null): void {
    this.value.set(value || null);
    this.error.set(null);
  }

  registerOnChange(fn: (value: string | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  openPicker(fileInput: HTMLInputElement) {
    fileInput.click();
  }

  onFileSelected(event: Event) {
    const fileInput = event.target as HTMLInputElement;
    const file = fileInput.files?.[0];
    fileInput.value = ''; // dozvoljava da se isti fajl izabere ponovo

    if (file) {
      this.upload(file);
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    if (!this.disabled()) {
      this.dragging.set(true);
    }
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.dragging.set(false);

    const file = event.dataTransfer?.files?.[0];
    if (file && !this.disabled()) {
      this.upload(file);
    }
  }

  remove() {
    this.value.set(null);
    this.error.set(null);
    this.onChange(null);
    this.onTouched();
  }

  private upload(file: File) {
    const problem = validateImageFile(file);
    if (problem) {
      this.error.set(problem);
      return;
    }

    this.error.set(null);
    this.uploading.set(true);

    this.uploadService.uploadFile(file).subscribe({
      next: result => {
        this.value.set(result.url);
        this.uploading.set(false);
        this.onChange(result.url);
        this.onTouched();
      },
      error: err => {
        this.uploading.set(false);
        this.error.set(uploadErrorMessage(err));
      }
    });
  }
}