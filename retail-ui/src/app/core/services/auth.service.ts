import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  username: string;
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = `${environment.apiUrl}/Auth`;
  private currentUserSubject = new BehaviorSubject<string | null>(null);
  private tokenSubject = new BehaviorSubject<string | null>(null);
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);

  public currentUser$ = this.currentUserSubject.asObservable();
  public token$ = this.tokenSubject.asObservable();
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private readonly http: HttpClient) {
    this.initializeAuth();
  }

  private initializeAuth(): void {
    const token = localStorage.getItem('auth_token');
    const userStr = localStorage.getItem('current_user');
    
    if (token && userStr) {
      try {
        const user = JSON.parse(userStr);
        this.tokenSubject.next(token);
        this.currentUserSubject.next(user);
        this.isAuthenticatedSubject.next(true);
      } catch (error) {
        this.clearAuth();
      }
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, credentials)
      .pipe(
        tap(response => {
          this.setAuth(response.username, response.token);
        }),
        catchError(error => {
          console.error('Login error:', error);
          return throwError(() => error);
        })
      );
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/logout`, {})
      .pipe(
        tap(() => {
          this.clearAuth();
        }),
        catchError(error => {
          // Even if logout fails on server, clear local auth
          this.clearAuth();
          return of(void 0);
        })
      );
  }

  refreshToken(): Observable<LoginResponse> {
    const token = this.getToken();
    if (!token) {
      return throwError(() => new Error('No token to refresh'));
    }

    return this.http.post<LoginResponse>(`${this.baseUrl}/refresh`, { token })
      .pipe(
        tap(response => {
          this.setAuth(response.username, response.token);
        }),
        catchError(error => {
          this.clearAuth();
          return throwError(() => error);
        })
      );
  }

  getCurrentUser(): string | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return this.tokenSubject.value;
  }

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  private setAuth(username: string, token: string): void {
    localStorage.setItem('auth_token', token);
    localStorage.setItem('current_user', username);
    
    this.tokenSubject.next(token);
    this.currentUserSubject.next(username);
    this.isAuthenticatedSubject.next(true);
  }

  private clearAuth(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('current_user');
    
    this.tokenSubject.next(null);
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }
}
