import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { UserModel } from '../../models/user.model';
import { environment } from '../../../../../environments/environment';
import { AuthModel } from '../../models/auth.model';
import { RegistrationUserModel } from '../../models/registration-user.model';
import { ResetPasswordModel } from '../../models/reset-password.model';
import { ConfirmEmailModel } from '../../models/confirm-email.model';
import { ConfirmEmailAndSetPasswordModel } from '../../models/confirm-email-and-set-password.model';
import { RefreshTokenModel } from '../../models/refresh-token.model';
import { ChangeEmailModel } from '../../models/change-email.model';
import { ChangePasswordModel } from '../../../account/models/change-password.model';
import { TwoFactorModel } from '../../models/two-factor.model';
import { User } from 'src/app/pages/user/models/user.model';

const API_AUTHENTICATION_URL = `${environment.apiUrl}/${environment.apiAuthenticationRelativeUrl}`;

@Injectable({
  providedIn: 'root',
})
export class AuthHTTPService {
  constructor(private http: HttpClient) {}

  // public methods
  changeEmail(newEmail: string): Observable<boolean> {
    return this.http.post<boolean>(`${API_AUTHENTICATION_URL}/change-email`, {
      newEmail
    });
  }

  changePassword(newPassword: ChangePasswordModel): Observable<boolean> {
    return this.http.post<boolean>(`${API_AUTHENTICATION_URL}/change-password`, newPassword);
  }

  confirmEmail(confirmEmail: ConfirmEmailModel): Observable<boolean> {
    return this.http.post<boolean>(`${API_AUTHENTICATION_URL}/verify-email`, confirmEmail);
  }

  confirmEmailAndSetPassword(confirmEmailAndSetPassword: ConfirmEmailAndSetPasswordModel): Observable<boolean> {
    return this.http.post<boolean>(`${API_AUTHENTICATION_URL}/verify-email-and-set-password`, confirmEmailAndSetPassword);
  }

  confirmNewEmail(confirmNewEmail: ChangeEmailModel): Observable<boolean> {
    return this.http.post<boolean>(`${API_AUTHENTICATION_URL}/verify-new-email`, confirmNewEmail);
  }

  createUser(user: User): Observable<User> {
    return this.http.post<User>(`${API_AUTHENTICATION_URL}/create-user`, user).pipe(map(user => new User(user)));
  }

  deactivate(): Observable<boolean> {
    return this.http.delete<boolean>(`${API_AUTHENTICATION_URL}/deactivate`, {});
  }

  forgotPassword(email: string): Observable<boolean> {
    return this.http.post<boolean>(`${API_AUTHENTICATION_URL}/forgot-password`, {
      email,
    });
  }

  getUserByToken(): Observable<UserModel> {
    return this.http.get<UserModel>(`${API_AUTHENTICATION_URL}/me`);
  }

  login(email: string, password: string): Observable<any> {
    return this.http.post<AuthModel>(`${API_AUTHENTICATION_URL}/login`, {
      email,
      password,
    });
  }

  refresh(refreshToken: RefreshTokenModel): Observable<UserModel> {
    return this.http.post<UserModel>(`${API_AUTHENTICATION_URL}/refresh-token`, refreshToken);
  }

  registerUser(user: RegistrationUserModel): Observable<UserModel> {
    return this.http.post<UserModel>(`${API_AUTHENTICATION_URL}/register`, user);
  }

  resendVerifyEmail(email: string): Observable<boolean> {
    return this.http.post<boolean>(`${API_AUTHENTICATION_URL}/resend-verify-email`, {
      email,
    });
  }

  resetPassword(newPassword: ResetPasswordModel): Observable<boolean> {
    return this.http.post<boolean>(`${API_AUTHENTICATION_URL}/reset-password`, newPassword);
  }

  setTwoFactorEnabled(enabled: boolean): Observable<boolean> {
    return this.http.post<boolean>(`${API_AUTHENTICATION_URL}/set-two-factor-enabled`, {
      enabled
    });
  }

  twoFactor(twoFactor: TwoFactorModel): Observable<any> {
    return this.http.post<boolean>(`${API_AUTHENTICATION_URL}/two-factor`, twoFactor);
  }
}