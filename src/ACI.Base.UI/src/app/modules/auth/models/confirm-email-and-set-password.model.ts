export class ConfirmEmailAndSetPasswordModel {
    confirmationPassword: string;  
    email: string;
    emailToken: string;
    password: string;
    passwordToken: string;
  
    set(_confirmEmailAndSetPassword: unknown) {
      const confirmEmail = _confirmEmailAndSetPassword as ConfirmEmailAndSetPasswordModel;
      this.confirmationPassword = confirmEmail.confirmationPassword || '';
      this.email = confirmEmail.email || '';
      this.emailToken = confirmEmail.emailToken || '';
      this.password = confirmEmail.password || '';  
      this.passwordToken = confirmEmail.passwordToken || '';
    }
  }
  