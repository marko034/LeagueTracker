import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, catchError, finalize, firstValueFrom, map, of, shareReplay, tap } from 'rxjs';
import { RegisterRequest, LoginRequest, AuthResult } from '../models/auth';

// setTimeout ne podržava delay veći od ~24.8 dana
const MAX_TIMEOUT_MS = 2_147_483_647;

function getTokenExpiry(token: string): number | null {
  try {
    const payload = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const claims = JSON.parse(atob(payload));
    return typeof claims.exp === 'number' ? claims.exp * 1000 : null;
  } catch {
    return null;
  }
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = 'http://localhost:5296/api/Auth';

  // withCredentials: browser šalje i prima cookie i između različitih portova
  private options = { withCredentials: true };

  private refreshTimer: ReturnType<typeof setTimeout> | null = null;
  private refreshInFlight: Observable<AuthResult> | null = null;

  private tokenSignal = signal<string | null>(null);
  private displayNameSignal = signal<string | null>(null);
  private roleSignal = signal<string | null>(null);

  isLoggedIn = computed(() => this.tokenSignal() !== null);
  displayName = this.displayNameSignal.asReadonly();
  role = this.roleSignal.asReadonly();
  isAdmin = computed(() => this.roleSignal() === 'Admin');

  constructor() {
    // ostaci stare implementacije (token se više ne čuva u localStorage)
    ['league_tracker_token', 'league_tracker_display_name', 'league_tracker_role']
      .forEach(key => localStorage.removeItem(key));
  }

  // Poziva se jednom pri pokretanju aplikacije: da li imamo važeći refresh cookie?
  initialize(): Promise<void> {
    return firstValueFrom(
      this.refresh().pipe(
        map(() => undefined),
        catchError(() => of(undefined))
      )
    );
  }

  getToken(): string | null {
    return this.tokenSignal();
  }

  register(dto: RegisterRequest) {
    return this.http
      .post<AuthResult>(`${this.apiUrl}/register`, dto, this.options)
      .pipe(tap(result => this.storeSession(result)));
  }

  login(dto: LoginRequest) {
    return this.http
      .post<AuthResult>(`${this.apiUrl}/login`, dto, this.options)
      .pipe(tap(result => this.storeSession(result)));
  }

  // Više istovremenih poziva deli jedan isti zahtev (refresh token je jednokratan!)
  refresh(): Observable<AuthResult> {
    if (!this.refreshInFlight) {
      this.refreshInFlight = this.http
        .post<AuthResult>(`${this.apiUrl}/refresh`, {}, this.options)
        .pipe(
          tap(result => this.storeSession(result)),
          finalize(() => (this.refreshInFlight = null)),
          shareReplay(1)
        );
    }

    return this.refreshInFlight;
  }

  logout(expired = false) {
    this.clearTimer();

    this.http.post(`${this.apiUrl}/logout`, {}, this.options).subscribe({ error: () => undefined });

    this.tokenSignal.set(null);
    this.displayNameSignal.set(null);
    this.roleSignal.set(null);

    this.router.navigate(['/login'], expired ? { queryParams: { expired: 1 } } : {});
  }

  private storeSession(result: AuthResult) {
    this.tokenSignal.set(result.token);
    this.displayNameSignal.set(result.displayName);
    this.roleSignal.set(result.role);

    this.scheduleRefresh(result.token);
  }

  // Obnavlja token malo pre isteka, da korisnik nikad ne naiđe na 401
  private scheduleRefresh(token: string) {
    this.clearTimer();

    const expiry = getTokenExpiry(token);
    if (expiry === null) {
      return;
    }

    const remaining = expiry - Date.now();
    const margin = Math.min(60_000, remaining / 2);
    const delay = Math.min(Math.max(remaining - margin, 0), MAX_TIMEOUT_MS);

    this.refreshTimer = setTimeout(() => {
      this.refresh().subscribe({ error: () => this.logout(true) });
    }, delay);
  }

  private clearTimer() {
    if (this.refreshTimer !== null) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = null;
    }
  }
}