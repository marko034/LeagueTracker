export const ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/png', 'image/webp'];
export const MAX_IMAGE_BYTES = 5 * 1024 * 1024;

// ista pravila kao na backend-u (UploadsController), da korisnik odmah dobije poruku
export function validateImageFile(file: File): string | null {
  if (!ALLOWED_IMAGE_TYPES.includes(file.type)) {
    return 'Dozvoljeni formati su JPG, PNG i WebP.';
  }

  if (file.size > MAX_IMAGE_BYTES) {
    return 'Slika je prevelika (najviše 5 MB).';
  }

  return null;
}

export function uploadErrorMessage(err: any): string {
  const body = err?.error;

  if (typeof body === 'string' && body) {
    return body; // UploadsController vraća običan tekst
  }

  return body?.error ?? 'Upload slike nije uspeo.';
}