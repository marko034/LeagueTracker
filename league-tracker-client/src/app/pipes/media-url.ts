import { Pipe, PipeTransform } from '@angular/core';
import { environment } from '../../environments/environment';

@Pipe({ name: 'mediaUrl' })
export class MediaUrlPipe implements PipeTransform {
  transform(path: string | null | undefined): string | null {
    if (!path) {
      return null;
    }

    // ako je negde ostala puna adresa, ne dodajemo ništa
    if (/^https?:\/\//i.test(path)) {
      return path;
    }

    return `${environment.mediaBaseUrl}${path}`;
  }
}