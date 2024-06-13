export class ConfirmEmailModel {
    email: string;
    token: string;
  
    set(_confirmEmail: unknown) {
      const confirmEmail = _confirmEmail as ConfirmEmailModel;
      this.email = confirmEmail.email || '';
      this.token = confirmEmail.token || '';
    }
  }
  