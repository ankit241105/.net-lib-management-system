import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, finalize, shareReplay } from 'rxjs';
import { API } from './models';

interface LoginResponse {
  token: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);

  private tokenSig = signal<string | null>(localStorage.getItem('token'));
  private usernameSig = signal<string | null>(localStorage.getItem('username'));

  readonly username = this.usernameSig.asReadonly();
  readonly isLoggedIn = computed(() => this.tokenSig() !== null);
  readonly role = computed(() => this.decodeRole(this.tokenSig()));
  readonly isAdmin = computed(() => this.role() === 'Admin');
  readonly isMember = computed(() => this.role() === 'Member');
  readonly isUser = computed(() => this.role() === 'User');

  login(username: string, password: string): Observable<LoginResponse> {
    // withCredentials so the Set-Cookie refresh token is stored by the browser.
    return this.http
      .post<LoginResponse>(`${API}/api/auth/login`, { username, password }, { withCredentials: true })
      .pipe(tap((res) => this.setSession(res.token, username)));
  }

  // De-duplicated silent refresh: concurrent 401s share ONE refresh request so the
  // rotating refresh token isn't spent twice.
  private refresh$?: Observable<LoginResponse>;
  refresh(): Observable<LoginResponse> {
    if (!this.refresh$) {
      this.refresh$ = this.http
        .post<LoginResponse>(`${API}/api/auth/refresh`, {}, { withCredentials: true })
        .pipe(
          tap((res) => this.setToken(res.token)),
          finalize(() => (this.refresh$ = undefined)),
          shareReplay(1)
        );
    }
    return this.refresh$;
  }

  // Public self-registration -> creates a "User" (role decided server-side).
  register(username: string, email: string, password: string): Observable<string> {
    return this.http.post(`${API}/api/auth/register`,
      { username, email, password },
      { responseType: 'text' });
  }

  registerAdmin(username: string, password: string): Observable<string> {
    return this.http.post(`${API}/api/auth/register-admin`,
      { username, password },
      { responseType: 'text' });
  }

  logout(): void {
    // Best-effort server-side revoke of the refresh token, then clear local state.
    this.http.post(`${API}/api/auth/logout`, {}, { withCredentials: true })
      .subscribe({ next: () => {}, error: () => {} });
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    this.tokenSig.set(null);
    this.usernameSig.set(null);
  }

  getToken(): string | null {
    return this.tokenSig();
  }

  private setSession(token: string, username: string): void {
    localStorage.setItem('username', username);
    this.usernameSig.set(username);
    this.setToken(token);
  }

  private setToken(token: string): void {
    localStorage.setItem('token', token);
    this.tokenSig.set(token);
  }

  private decodeRole(token: string | null): string | null {
    if (!token) return null;
    try {
      const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
      const payload = JSON.parse(atob(base64));
      return (
        payload['role'] ||
        payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
        null
      );
    } catch {
      return null;
    }
  }
}
