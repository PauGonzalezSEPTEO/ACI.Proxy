import { Injectable, OnDestroy } from '@angular/core';
import { Observable, BehaviorSubject, of, Subscription } from 'rxjs';
import { map, catchError, switchMap, finalize } from 'rxjs/operators';
import { UserModel } from '../models/user.model';
import { AuthModel } from '../models/auth.model';
import { AuthHTTPService } from './auth-http';
import { Router } from '@angular/router';
import { RegistrationUserModel } from '../models/registration-user.model';
import { ResetPasswordModel } from '../models/reset-password.model';
import { ConfirmEmailModel } from '../models/confirm-email.model';
import { ConfirmEmailAndSetPasswordModel } from '../models/confirm-email-and-set-password.model';
import { ChangeEmailModel } from '../models/change-email.model';
import { ChangePasswordModel } from '../../account/models/change-password.model';
import { LocalStorage } from '../../../helpers/local-storage';
import { TwoFactorModel } from '../models/two-factor.model';
import { LoginModel } from '../models/login.model';
import { LockedOutModel } from '../models/locked-out.model';
import { AccountService } from '../../account/services/account.service';
import { jwtDecode, JwtPayload } from 'jwt-decode';

export type UserType = UserModel | void;

interface MicrosoftJwtPayload extends JwtPayload {
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService implements OnDestroy {
  // private fields
  private unsubscribe: Subscription[] = [];

  // public fields
  currentUser$: Observable<UserType>;
  isLoading$: Observable<boolean>;
  currentUserSubject: BehaviorSubject<UserType>;
  isLoadingSubject: BehaviorSubject<boolean>;

  get currentUserValue(): UserType {
    return this.currentUserSubject.value;
  }

  set currentUserValue(user: UserType) {
    this.currentUserSubject.next(user);
  }

  constructor(
    private authHttpService: AuthHTTPService,
    private accountService: AccountService,
    private router: Router,
    private localStorage: LocalStorage
  ) {
    this.isLoadingSubject = new BehaviorSubject<boolean>(false);
    this.currentUserSubject = new BehaviorSubject<UserType>(undefined);
    this.currentUser$ = this.currentUserSubject.asObservable();
    this.isLoading$ = this.isLoadingSubject.asObservable();
    this.refreshUser();
    this.accountService.account.subscribe(account => {
      this.refreshUser();
    });    
  }

  // public methods
  changeEmail(newEmail: string): Observable<boolean> {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .changeEmail(newEmail)
      .pipe(
        catchError((err) => {
          console.error('err', err);
          return of(err);
        }),
        finalize(() => this.isLoadingSubject.next(false))
      );
  }

  changePassword(newPassword: ChangePasswordModel): Observable<boolean> {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .changePassword(newPassword)
      .pipe(
        catchError((err) => {
          console.error('err', err);
          return of(err);
        }),
        finalize(() => this.isLoadingSubject.next(false))
      );
  }

  confirmEmail(confirmEmail: ConfirmEmailModel): Observable<boolean> {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .confirmEmail(confirmEmail)
      .pipe(
        catchError((err) => {
          console.error('err', err);
          return of(err);
        }),
        finalize(() => this.isLoadingSubject.next(false))
      );
  }

  confirmEmailAndSetPassword(confirmEmailAndSetPassword: ConfirmEmailAndSetPasswordModel): Observable<boolean> {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .confirmEmailAndSetPassword(confirmEmailAndSetPassword)
      .pipe(
        catchError((err) => {
          console.error('err', err);
          return of(err);
        }),
        finalize(() => this.isLoadingSubject.next(false))
      );
  }

  confirmNewEmail(confirmNewEmail: ChangeEmailModel): Observable<boolean> {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .confirmNewEmail(confirmNewEmail)
      .pipe(
        catchError((err) => {
          console.error('err', err);
          return of(err);
        }),
        finalize(() => this.isLoadingSubject.next(false))
      );
  }

  deactivate(): Observable<boolean> {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .deactivate()
      .pipe(
        catchError((err) => {
          console.error('err', err);
          return of(err);
        }),
        finalize(() => this.isLoadingSubject.next(false))
      );
  }

  forgotPassword(email: string): Observable<boolean> {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .forgotPassword(email)
      .pipe(
        catchError((err) => {
          console.error('err', err);
          return of(err);
        }),
        finalize(() => this.isLoadingSubject.next(false))
      );
  }

  getUserByToken(): Observable<UserType> {
    const auth = this.localStorage.getAuthFromLocalStorage();
    if (!auth || !auth.accessToken) {
      return of(undefined);
    }
    this.isLoadingSubject.next(true);
    return this.authHttpService.getUserByToken().pipe(
      map((user: UserType) => {
        if (user) {
          this.currentUserSubject.next(user);
        } else {
          this.logout();
        }
        return user;
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  login(email: string, password: string): Observable<UserType | LockedOutModel> {
    this.isLoadingSubject.next(true);
    return this.authHttpService.login(email, password).pipe(
      map((login: LoginModel) => {
        if (login.isTwoFactorVerificationRequired) {
          this.router.navigate(['/auth/two-factor'], {
            queryParams: {
              email: login.email,
              provider: login.twoFactorProvider
            },
          });
        } else if (login.isLockedOut || (login.accessFailedCount > 0)) {
          var locketOut = new LockedOutModel();
          locketOut.setLockedOut(login);
          return locketOut;
        } else {
          const result = this.localStorage.setAuthFromLocalStorage(login);
          return result;
        }
      }),
      switchMap((data: any) => {
        if ((data !== undefined) && (data instanceof LockedOutModel)) {
          return of(data);
        } else {
          return this.getUserByToken();
        }
      }),
      catchError((err) => {
        console.error('err', err);
        return of(err);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  logout() {
    this.localStorage.removeAuthFromLocalStorage();
    this.router.navigate(['/auth/login'], {
      queryParams: {},
    });
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }

  refreshUser() {
    const subscr = this.getUserByToken().subscribe();
    this.unsubscribe.push(subscr);
  }

  registration(user: RegistrationUserModel): Observable<any> {
    this.isLoadingSubject.next(true);
    return this.authHttpService.registerUser(user).pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  resendVerifyEmail(email: string) {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .resendVerifyEmail(email)
      .pipe(
        catchError((err) => {
          console.error('err', err);
          return of(err);
        }),
        finalize(() => this.isLoadingSubject.next(false))
      );
  }

  resetPassword(newPassword: ResetPasswordModel): Observable<boolean> {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .resetPassword(newPassword)
      .pipe(
        catchError((err) => {
          console.error('err', err);
          return of(err);
        }),
        finalize(() => this.isLoadingSubject.next(false))
      );
  }

  setTwoFactorEnabled(enabled: boolean): Observable<boolean> {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .setTwoFactorEnabled(enabled)
      .pipe(
        catchError((err) => {
          console.error('err', err);
          return of(err);
        }),
        finalize(() => this.isLoadingSubject.next(false))
      );
  }

  twoFactor(twoFactor: TwoFactorModel): Observable<UserType> {
    this.isLoadingSubject.next(true);
    return this.authHttpService.twoFactor(twoFactor).pipe(
      map((auth: AuthModel) => {
        const result = this.localStorage.setAuthFromLocalStorage(auth);
        return result;
      }),
      switchMap(() => this.getUserByToken()),
      catchError((err) => {
        console.error('err', err);
        return of(err);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  userHasRole(roles: string[]): boolean {
    const auth = this.localStorage.getAuthFromLocalStorage();
    if (auth && auth.accessToken) {
      const token = auth.accessToken;
      const decodedToken = jwtDecode<MicrosoftJwtPayload>(token);
      const userRoles = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
      if (userRoles) {
        return roles.some(role => userRoles.includes(role));
      }
    }
    return false;
  }
}