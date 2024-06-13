import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse, HttpClient } from '@angular/common/http';
import { from, Observable, throwError } from "rxjs";
import { environment } from 'src/environments/environment';
import { catchError, switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthModel } from '../modules/auth/models/auth.model';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LocalStorage } from './local-storage';

const API_AUTHENTICATION_URL = `${environment.apiUrl}/${environment.apiAuthenticationRelativeUrl}`;

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private jwtHelper: JwtHelperService, private router: Router, private http: HttpClient, private localStorage: LocalStorage) { }

  private handleBadRequest = (error: HttpErrorResponse): string => {
    if ((this.router.url === '/auth/registration') ||
      this.router.url.startsWith('/auth/login') ||
      this.router.url.startsWith('/auth/confirm-email') ||
      this.router.url.startsWith('/auth/confirm-email-and-set-password') ||
      this.router.url.startsWith('/auth/deactivate') ||
      this.router.url.startsWith('/auth/forgot-password') ||
      this.router.url.startsWith('/auth/reset-password') ||
      this.router.url.startsWith('/auth/change-email') ||
      this.router.url.startsWith('/auth/change-password') ||
      this.router.url.startsWith('/auth/set-two-factor-enabled') ||
      this.router.url.startsWith('/auth/two-factor') ||
      this.router.url.startsWith('/crafted/account/overview') ||
      this.router.url.startsWith('/crafted/account/settings')
    ) {
      let message = '';
      let values = [];
      if (error.error.errors) {
        values = Object.values(error.error.errors);
      } else {
        values = Object.values(error.error);
      }
      values.map((m: any) => {
        if (m.description) {
          message += m.description + '<br>';
        } else {
          message += m + '<br>';
        }
      })
      return message.slice(0, -4);
    }
    else {
      return error.error ? error.error : error.message;
    }
  }

  private handleError = (error: HttpErrorResponse) => {
    if (error.status === 404) {
      return this.handleNotFound(error);
    }
    else if (error.status === 401) {
      return this.handleUnauthorized(error);
    }
    else if (error.status === 400) {
      return this.handleBadRequest(error);
    }
  }
  private handleNotFound = (error: HttpErrorResponse): string => {
    this.router.navigate(['/404']);
    return error.message;
  }
  private handleUnauthorized = (error: HttpErrorResponse): string => {
    return error.error ? error.error : error.message;
  }

  public async refreshToken(): Promise<AuthModel> {
    var refreshToken = this.localStorage.getAuthFromLocalStorage();
    return await new Promise<AuthModel>((resolve, reject) => {
      this.http.post<AuthModel>(`${API_AUTHENTICATION_URL}/refresh-token`, refreshToken).subscribe({
        next: (auth: AuthModel) => {
          resolve(auth);
          this.localStorage.setAuthFromLocalStorage(auth);
        },
        error: (_) => {
          reject();
        }
      });
    });
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const isApiUrl = request.url.startsWith(`${environment.apiUrl}/`) &&
      (
        !request.url.startsWith(`${environment.apiUrl}/${environment.apiAuthenticationRelativeUrl}`) ||
        (
          request.url.startsWith(`${environment.apiUrl}/${environment.apiAuthenticationRelativeUrl}/me`) ||
          request.url.startsWith(`${environment.apiUrl}/${environment.apiAuthenticationRelativeUrl}/change-email`) ||
          request.url.startsWith(`${environment.apiUrl}/${environment.apiAuthenticationRelativeUrl}/change-password`) ||
          request.url.startsWith(`${environment.apiUrl}/${environment.apiAuthenticationRelativeUrl}/set-two-factor-enabled`) ||
          request.url.startsWith(`${environment.apiUrl}/${environment.apiAuthenticationRelativeUrl}/deactivate`)
        )
      );
    if (isApiUrl) {
      let error: boolean = false;
      var refreshToken = this.localStorage.getAuthFromLocalStorage();
      if (refreshToken && refreshToken.accessToken && this.jwtHelper.isTokenExpired(refreshToken.accessToken)) {
        return from(this.refreshToken().catch(_ => {
          error = true;
        })).pipe(
          switchMap(data => {
            if (!error) {
              if (data && data.accessToken) {
                request = request.clone({
                  setHeaders: { Authorization: `Bearer ${data.accessToken}` }
                });
              }
            }          
            return next.handle(request).pipe(
              catchError((error: HttpErrorResponse) => {
                let errorMessage = this.handleError(error);
                return throwError(() => new Error(errorMessage));
              })
            );
          })
        );
      }
      if (!error) {
        const auth = this.localStorage.getAuthFromLocalStorage();
        if (!(!auth || !auth.accessToken)) {
          request = request.clone({
            setHeaders: { Authorization: `Bearer ${auth.accessToken}` }
          });
        }
      }
    }
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = this.handleError(error);
        return throwError(() => new Error(errorMessage));
      })
    );
  }
}