import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class  Upload {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5296/api/uploads';

  uploadFile(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ url: string }>(`${this.apiUrl}/image`, formData);
  }
}
