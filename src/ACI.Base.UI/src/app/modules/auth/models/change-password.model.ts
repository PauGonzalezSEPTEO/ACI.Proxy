export class ChangePasswordModel {
    currentPassword: string;
    newConfirmationPassword: string;
    newPassword: string;
  
    set(changePasswordModel: ChangePasswordModel) {
      this.currentPassword = changePasswordModel.currentPassword;
      this.newConfirmationPassword = changePasswordModel.newConfirmationPassword;
      this.newPassword = changePasswordModel.newPassword;
    }
  }  