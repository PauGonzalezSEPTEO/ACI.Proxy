export class ResetPasswordModel {
    confirmationPassword: string;
    email: string;
    password: string;
    token: string;
  
    set(_resetPassword: unknown) {
      const resetPassword = _resetPassword as ResetPasswordModel;
      this.confirmationPassword = resetPassword.confirmationPassword || '';
      this.email = resetPassword.email || '';
      this.password = resetPassword.password || '';
      this.token = resetPassword.token || '';
    }
  }
  