export class ChangeEmailModel {
    email: string;
    newEmail: string;
    token: string;
  
    set(_changeEmail: unknown) {
      const changeEmail = _changeEmail as ChangeEmailModel;
      this.email = changeEmail.email || '';
      this.newEmail = changeEmail.newEmail || '';
      this.token = changeEmail.token || '';
    }
  }
  