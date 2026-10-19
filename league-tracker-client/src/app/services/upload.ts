import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class  UploadService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/api/Uploads`;

  uploadFile(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ path: string }>(`${this.apiUrl}/image`, formData);
  }
}
