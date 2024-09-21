export class TwoFactorModel {
    email: string;
    provider: string;
    token: string;
  
    set(_twoFactor: unknown) {
      const twoFactor = _twoFactor as TwoFactorModel;
      this.email = twoFactor.email || '';
      this.provider = twoFactor.provider || '';
      this.token = twoFactor.token || '';
    }
  }  