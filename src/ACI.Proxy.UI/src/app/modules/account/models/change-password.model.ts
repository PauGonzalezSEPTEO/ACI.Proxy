export class ChangePasswordModel {
    currentPassword: string;
    newConfirmationPassword: string;
    newPassword: string;
  
    set(_changePasswordModel: unknown) {
      const changePasswordModel = _changePasswordModel as ChangePasswordModel;
      this.currentPassword = changePasswordModel.currentPassword;
      this.newConfirmationPassword = changePasswordModel.newConfirmationPassword;
      this.newPassword = changePasswordModel.newPassword;
    }
  }  