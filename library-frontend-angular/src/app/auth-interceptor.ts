import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth';

// Functional interceptor on EVERY HttpClient request:
// 1. Attaches the Bearer access token.
// 2. On a 401, silently refreshes the access token (via the refresh cookie) and retries
//    the request once. If the refresh fails, logs out and sends the user to /login.
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  // The auth endpoints manage their own tokens/cookies — don't attach a header or
  // try to refresh when one of them fails.
  const isAuthCall = req.url.includes('/api/auth/');
  const token = auth.getToken();

  const authReq = token && !isAuthCall
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((err) => {
      if (err.status === 401 && !isAuthCall) {
        return auth.refresh().pipe(
          switchMap(() =>
            next(req.clone({ setHeaders: { Authorization: `Bearer ${auth.getToken()}` } }))
          ),
          catchError((refreshErr) => {
            auth.logout();
            router.navigate(['/login']);
            return throwError(() => refreshErr);
          })
        );
      }
      return throwError(() => err);
    })
  );
};
