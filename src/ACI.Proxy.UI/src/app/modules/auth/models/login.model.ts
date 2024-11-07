import { AuthModel } from "./auth.model";

export class LoginModel extends AuthModel {
  accessFailedCount: number;
  email: string;
  isLockedOut: boolean;
  isTwoFactorVerificationRequired: boolean;
  twoFactorProvider: string;

  setLogin(login: LoginModel) {
    this.setAuth(login);
    this.accessFailedCount = login.accessFailedCount;
    this.email = login.email;
    this.isLockedOut = login.isLockedOut;
    this.isTwoFactorVerificationRequired = login.isTwoFactorVerificationRequired;
    this.twoFactorProvider = login.twoFactorProvider;
  }
}
