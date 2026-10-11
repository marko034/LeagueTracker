import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth-service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  const withToken = (request: HttpRequest<unknown>, token: string | null) =>
    token ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : request;

  const isAuthEndpoint = req.url.includes('/api/Auth/');
  const token = authService.getToken();

  return next(withToken(req, token)).pipe(
    catchError((error: HttpErrorResponse) => {
      // Obnova ima smisla samo ako smo poslali token, a odbijen je (401) na običnom endpoint-u
      if (error.status !== 401 || isAuthEndpoint || !token) {
        return throwError(() => error);
      }

      return authService.refresh().pipe(
        catchError(() => {
          authService.logout(true);
          return throwError(() => error);
        }),
        switchMap(result => next(withToken(req, result.token)))
      );
    })
  );
};