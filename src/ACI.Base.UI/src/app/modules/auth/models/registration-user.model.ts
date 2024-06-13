export class RegistrationUserModel {
    confirmationPassword: string;
    email: string;
    firstname: string;
    lastname: string;
    password: string;
  
    set(_registrationUser: unknown) {
      const registrationUser = _registrationUser as RegistrationUserModel;
      this.confirmationPassword = registrationUser.confirmationPassword || '';
      this.email = registrationUser.email || '';
      this.firstname = registrationUser.firstname || '';
      this.lastname = registrationUser.lastname || '';
      this.password = registrationUser.password || '';
    }
  }  