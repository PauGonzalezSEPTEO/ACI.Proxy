import { AbstractControl } from '@angular/forms';

export class ConfirmationPasswordValidator {
  /**
   * Check matching password with confirm password
   * @param control AbstractControl
   */
  static MatchPassword(control: AbstractControl): void {
    const password = control.get('password')?.value;
    const confirmationPassword = control.get('confirmationPassword')?.value;

    if (password !== confirmationPassword) {
      control.get('confirmationPassword')?.setErrors({ ConfirmationPassword: true });
    }
  }

  /**
 * Check matching new password with confirm new password
 * @param control AbstractControl
 */
  static MatchNewPassword(control: AbstractControl): void {
    const newPassword = control.get('newPassword')?.value;
    const newConfirmationPassword = control.get('newConfirmationPassword')?.value;

    if (newPassword !== newConfirmationPassword) {
      control.get('newConfirmationPassword')?.setErrors({ newConfirmationPassword: true });
    }
  }
}